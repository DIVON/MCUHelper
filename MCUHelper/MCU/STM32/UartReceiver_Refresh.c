/*
 * file: UartReceiver_Refresh.c
 *
 * Implementation for UartReceiver_Refresh
 *
 * $Author:  $
 * $Date: $
 * $Revision:  $
 *
 */

/**** INCLUDES ***********************************************************************************/
#include "UartReceiver.h"
#include "stm32f4xx_hal.h"
#include <string.h>
/**** END OF INCLUDES ****************************************************************************/


/**** MACROS *************************************************************************************/
#define START_SYMBOL        0xA5u
#define STOP_SYMBOL         0x0D

#define READ_COMMAND        0xB3u
#define WRITE_COMMAND       0xC4u

#define ADDRESS_COUNT       20u
#define RECEIVE_MESSAGE_LEN (3u + ADDRESS_COUNT * 4u)
#define WRITE_MESSAGE_LEN   12u
#define SEND_MESSAGE_LEN     (3u + ADDRESS_COUNT * 8u)
#define RECEIVE_BUF_LEN     180u
#define SECTIONS_COUNT      2u
/**** END OF MACROS ******************************************************************************/


/**** TYPE DEFINITIONS ***************************************************************************/
typedef enum
{
    INIT,
    WAIT_RECEIVE,
    WAIT_SEND
} MachineState;

typedef struct
{
    uint32 startAddress;
    uint32 endAddress;
} MemorySection;

/**** END OF TYPE DEFINITIONS ********************************************************************/


/**** VARIABLES **********************************************************************************/
extern UART_HandleTypeDef huart1;

const MemorySection memorySections[SECTIONS_COUNT] =
{
    { 0x20000000u, 0x20000000u + 128 * 1024u },
    { 0x10000000u, 0x10000000u + 64  * 1024u }
};

STATIC_GLOBAL MachineState state = INIT;

STATIC_GLOBAL uint8_t receiveBuffer[RECEIVE_BUF_LEN];
STATIC_GLOBAL uint8_t sendBuffer[SEND_MESSAGE_LEN];

/**** END OF VARIABLES ***************************************************************************/


/**** LOCAL FUNCTION DECLARATIONS ****************************************************************/

/**** END OF LOCAL FUNCTION DECLARATIONS *********************************************************/


/**** LOCAL FUNCTION DEFINITIONS *****************************************************************/
static void PutValue(uint32 value, uint32 address, uint32 index)
{
    uint32 *sendBuffAddress = (uint32*)&sendBuffer[index * 8u + 2u];
    *sendBuffAddress = address;
    sendBuffAddress++;
    *sendBuffAddress = value;
}

static volatile uint32 lastAddr;
boolean ParceBuffer(uint8_t *buf, uint32 bufLen)
{
    boolean found = FALSE;
    for (uint32 i = 0; i <= bufLen - RECEIVE_MESSAGE_LEN; i++)
    {
        if ((START_SYMBOL == buf[i]) && (STOP_SYMBOL == buf[i + RECEIVE_MESSAGE_LEN - 1]))
        {
            if (READ_COMMAND == buf[i + 1])
            {
                for (uint32 addrIndex = 0; addrIndex < ADDRESS_COUNT; addrIndex++)
                {
                    uint32 currentAddrFound = FALSE;
                    uint32 address = buf[i + 2u + addrIndex * 4u] + (buf[i + 3u + addrIndex * 4u] << 8) + (buf[i + 4u + addrIndex * 4u] << 16) + (buf[i + 5u + addrIndex * 4u] << 24);
                    lastAddr = address;

                    for (uint32 sectionIndex = 0; sectionIndex < SECTIONS_COUNT; sectionIndex++)
                    {
                        if ((address >= memorySections[sectionIndex].startAddress) && (address <= memorySections[sectionIndex].endAddress))
                        {
                            uint32 data = *(uint32*)(address);
                            PutValue(data, address, addrIndex);
                            found = TRUE;
                            currentAddrFound = TRUE;
                            break;
                        }
                    }

                    if (currentAddrFound != TRUE)
                    {
                        /* Fill the rest of data with zeros */
                        for (; addrIndex < ADDRESS_COUNT; addrIndex++)
                        {
                            PutValue(0, 0, addrIndex);
                        }
                        break;
                    }
                }
                return TRUE;
            }
        }
    }


    return found;
}

boolean ParceBufferForWrite(uint8_t *buf, uint32 bufLen)
{
    for (uint32 i = 0; i < bufLen - WRITE_MESSAGE_LEN; i++)
    {
        if ((START_SYMBOL == buf[i]) && (STOP_SYMBOL == buf[i + WRITE_MESSAGE_LEN - 1]))
        {
            if (WRITE_COMMAND == buf[i + 1])
            {
                uint32 address = buf[i + 2u] + (buf[i + 3u] << 8) + (buf[i + 4u] << 16) + (buf[i + 5u] << 24);
                uint32 value   = buf[i + 7u] + (buf[i + 8u] << 8) + (buf[i + 9u] << 16) + (buf[i + 10u] << 24);
                uint8 count = buf[i + 6u];
                for (uint32 sectionIndex = 0; sectionIndex < SECTIONS_COUNT; sectionIndex++)
                {
                    if ((address >= memorySections[sectionIndex].startAddress) && (address <= memorySections[sectionIndex].endAddress))
                    {
                        switch (count)
                        {
                            case 1:
                            {
                                *(uint8*)(address) = value & 0xFF;
                                break;
                            }
                            case 2:
                            {
                                *(uint16*)(address) = value & 0xFFFF;
                                break;
                            }
                            case 4:
                            {
                                *(uint32*)(address) = value & 0xFFFFFFFF;
                                break;
                            }
                        }
                        return TRUE;
                    }
                }
            }
        }
    }


    return FALSE;
}
/**** END OF LOCAL FUNCTION DEFINITIONS **********************************************************/


/**** GLOBAL FUNCTION DEFINITIONS ****************************************************************/

/*
 * TODO: Fill runnables goals
 */
uint32 lastReceivedCount = 0;

void UartReceiver_ruRefresh(void)
{
    switch (state)
    {
        case INIT:
        {
            sendBuffer[0] = START_SYMBOL;
            sendBuffer[1] = READ_COMMAND;
            sendBuffer[SEND_MESSAGE_LEN - 1] = STOP_SYMBOL;

            memset(receiveBuffer, 0, RECEIVE_BUF_LEN);
            HAL_UART_Receive_DMA(&huart1, receiveBuffer, RECEIVE_BUF_LEN);
            state = WAIT_RECEIVE;
            break;
        }

        case WAIT_RECEIVE:
        {
            uint32 receivedBytes = RECEIVE_BUF_LEN - __HAL_DMA_GET_COUNTER(huart1.hdmarx);
            //uint32 receivedBytes = RECEIVE_BUF_LEN - huart1.RxXferCount;

            boolean parsingRead = FALSE;
            boolean parsingWrite = FALSE;

            if (lastReceivedCount != receivedBytes)
            {
                if (receivedBytes >= RECEIVE_MESSAGE_LEN)
                {
                    lastReceivedCount = receivedBytes;
                    parsingRead = ParceBuffer(receiveBuffer, receivedBytes);
                    if (TRUE == parsingRead)
                    {
                        HAL_UART_Transmit_DMA(&huart1, sendBuffer, SEND_MESSAGE_LEN);

                    }
                }

                if (receivedBytes >= WRITE_MESSAGE_LEN)
                {
                    parsingWrite = ParceBufferForWrite(receiveBuffer, receivedBytes);
                }

                if ((receivedBytes >= RECEIVE_BUF_LEN) || (parsingWrite != FALSE) || (parsingRead != FALSE))
                {
                    lastReceivedCount = 0;
                    HAL_UART_AbortReceive(&huart1);
                    memset(receiveBuffer, 0, RECEIVE_BUF_LEN);

                    HAL_UART_Receive_DMA(&huart1, receiveBuffer, RECEIVE_BUF_LEN);
                }
            }
            /* wait a message */
            break;
        }

        case WAIT_SEND:
        {
            break;
        }
    }
}

/**** END OF GLOBAL FUNCTION DEFINITIONS *********************************************************/

/* End of file */
