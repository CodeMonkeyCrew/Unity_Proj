using System;
using System.Collections;

namespace Assets.Scripts
{
    public class GameState
    {
        public Boolean StateChanged { get; set; }
        public int Lap { get; set; }
        public Item Item { get; set; }
        public Status Status1 { get; set; }
        public Status Status2 { get; set; }
        public Boolean InTrack { get; set; }
        public Position Position { get; set; }
        public GameStatus State { get; set; }

        public int PlayerNo { get; set; }

        public GameState()
        {
            StateChanged = true; //true to initialize HUD elements
            Lap = 0;
            Item = Item.NONE;
            Status1 = Status.NORMAL;
            Status2 = Status.NORMAL;
            InTrack = true;
            Position = Position.FIRST;
            PlayerNo = 0;
        }

        private const int LAP_START_BIT = 0;
        private const int LAP_END_BIT = 2;

        private const int ITEM_START_BIT = 3;
        private const int ITEM_END_BIT = 5;

        private const int BOOSTED_STATUS_BIT = 6;
        private const int SLOWED_STATUS_BIT = 7;
        private const int REVERSED_STATUS_BIT = 8;

        private const int IN_TRACK_BIT = 9;

        private const int RFU_START_BIT = 10;
        private const int RFU_END_BIT = 15;

        private const int PLAYER_OFFSET = 16;

        private const int GAME_STATUS_START_BIT = 32;
        private const int GAME_STATUS_END_BIT = 33;

        private bool isBoosted = false;
        private bool isSlowed = false;
        private bool isReversed = false;

        public void Update(Byte[] argRawNewState)
        {
            BitArray binaryRawNewState = new BitArray(argRawNewState);
            #region debug
            string bits = "";
            for (int i = 0; i < binaryRawNewState.Count; ++i)
            {
                if (binaryRawNewState[i])
                {
                    bits = bits + "1";
                }
                else
                {
                    bits = bits + "0";
                }
            }
            UnityEngine.Debug.Log("received bits: " + bits);
            #endregion

            int player0Laps = GetIntFromBits(binaryRawNewState, LAP_START_BIT, LAP_END_BIT);
            int player1Laps = GetIntFromBits(binaryRawNewState, LAP_START_BIT, LAP_END_BIT, PLAYER_OFFSET);

            HandleLaps(player0Laps, player1Laps);

            if (PlayerNo == 0)
            {
                Item = (Item)GetIntFromBits(binaryRawNewState, ITEM_START_BIT, ITEM_END_BIT);
                InTrack = GetIntFromBits(binaryRawNewState, IN_TRACK_BIT, IN_TRACK_BIT) == 1;
                isBoosted = GetIntFromBits(binaryRawNewState, BOOSTED_STATUS_BIT, BOOSTED_STATUS_BIT) == 1;
                isSlowed = GetIntFromBits(binaryRawNewState, SLOWED_STATUS_BIT, SLOWED_STATUS_BIT) == 1;
                isReversed = GetIntFromBits(binaryRawNewState, REVERSED_STATUS_BIT, REVERSED_STATUS_BIT) == 1;
            }
            else if (PlayerNo == 1)
            {
                Item = (Item)GetIntFromBits(binaryRawNewState, ITEM_START_BIT, ITEM_END_BIT, PLAYER_OFFSET);
                InTrack = GetIntFromBits(binaryRawNewState, IN_TRACK_BIT, IN_TRACK_BIT, PLAYER_OFFSET) == 1;
                isBoosted = GetIntFromBits(binaryRawNewState, BOOSTED_STATUS_BIT, BOOSTED_STATUS_BIT, PLAYER_OFFSET) == 1;
                isSlowed = GetIntFromBits(binaryRawNewState, SLOWED_STATUS_BIT, SLOWED_STATUS_BIT, PLAYER_OFFSET) == 1;
                isReversed = GetIntFromBits(binaryRawNewState, REVERSED_STATUS_BIT, REVERSED_STATUS_BIT, PLAYER_OFFSET) == 1;
            }
            SetStatus(isBoosted, isSlowed, isReversed);
            State = (GameStatus)GetIntFromBits(binaryRawNewState, GAME_STATUS_START_BIT, GAME_STATUS_END_BIT);

            StateChanged = true;
        }

        /// <summary>
        /// Converts the bits between argStartBit and argStopBit in argRawNewState to an Integer32
        /// </summary>
        /// <param name="argRawNewState">BitArray that is read from</param>
        /// <param name="argStartBit">argStartBit is the bit where is begins to read in argRawNewState</param>
        /// <param name="argStopBit">argStopBit is the bit where is stops to read in argRawNewState</param>
        /// <returns></returns>
        private int GetIntFromBits(BitArray argRawNewState, int argStartBit, int argStopBit, int argOffset = 0)
        {
            int startBit;
            int stopBit;

            startBit = argStartBit + argOffset;
            stopBit = argStopBit + argOffset;

            BitArray tempBitArray = new BitArray(stopBit - startBit + 1);
            int i = 0;
            int j = startBit;

            while (j <= stopBit)
            {
                tempBitArray[i] = argRawNewState[j];
                #region debug
                /*
                if (tempBitArray[i])
                {
                    UnityEngine.Debug.Log("1");
                } else
                {
                    UnityEngine.Debug.Log("0");
                }
                */
                #endregion
                ++i;
                ++j;
            }

            //convert to int
            int[] array = new int[1];
            tempBitArray.CopyTo(array, 0);
            return array[0];
        }

        private void HandleLaps(int player0Laps, int player1Laps)
        {
            if (PlayerNo == 0)
            {
                if (player0Laps > player1Laps)
                {
                    Position = Position.FIRST;
                }
                else if (player0Laps < player1Laps)
                {
                    Position = Position.SECOND;
                }

                if (player0Laps <= 3) //given max rounds is 3
                {
                    Lap = player0Laps;
                }
            }

            if (PlayerNo == 1)
            {
                if (player1Laps > player0Laps)
                {
                    Position = Position.FIRST;
                }
                else if (player1Laps < player0Laps)
                {
                    Position = Position.SECOND;
                }

                if (player1Laps <= 3) //given max rounds is 3
                {
                    Lap = player1Laps;
                }
            }
        }


        private void SetStatus(bool argBoosted, bool argSlowed, bool argReversed)
        {
            //use status1 for either boosted or slowed, as they cannot occure at the same time
            if (argBoosted)
            {
                Status1 = Status.BOOSTED;
            }
            else if (argSlowed)
            {
                Status1 = Status.SLOWED;
            }
            else if (!argBoosted && !argSlowed)
            {
                Status1 = Status.NORMAL;
            }

            //use status2 only for the reversed status
            if (argReversed)
            {
                Status2 = Status.REVERSED;
            }
            else
            {
                Status2 = Status.NORMAL;
            }
        }
    }
}


public enum Item
{
    NONE, BOOST, LIGHTNING, REVERSE
}

public enum Status
{
    NORMAL, BOOSTED, SLOWED, REVERSED
}

public enum Position
{
    FIRST, SECOND
}

public enum GameStatus
{
    READY, STARTING, IN_PROGRESS, FINISHED
}