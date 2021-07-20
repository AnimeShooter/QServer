using System;
using System.Security.Cryptography;

namespace TNL.Structures
{
    public enum ErrorCode
    {
        Success,
        InvalidSolution,
        InvalidServerNonce,
        InvalidClientNonce,
        InvalidPuzzleDifficulty,
        ErrorCodeCount,
    };

    public class ClientPuzzleManager
    {
        public const uint PuzzleRefreshTime = 30000;
        public const uint InitialPuzzleDifficulty = 17;
        public const uint MaxPuzzleDifficulty = 26;
        public const uint MaxSolutionComputeFragment = 30;
        public const uint SolutionFragmentIterations = 50000;

        public uint CurrentDifficulty { get; private set; }
        public int LastUpdateTime { get; private set; }
        public int LastTickTime { get; private set; }
        public Nonce CurrentNonce { get; private set; }
        public Nonce LastNonce { get; private set; }

        public ClientPuzzleManager()
        {
            CurrentDifficulty = InitialPuzzleDifficulty;
            LastUpdateTime = 0;
            LastTickTime = 0;

            CurrentNonce = new();
            CurrentNonce.GetRandom();

            LastNonce = new();
            LastNonce.GetRandom();
        }

        public void Tick(int currentTime)
        {
            if (LastTickTime == 0)
                LastTickTime = currentTime;

            var delta = currentTime - LastUpdateTime;
            if (delta <= PuzzleRefreshTime)
                return;

            LastUpdateTime = currentTime;
            LastNonce = CurrentNonce;

            CurrentNonce.GetRandom();
        }

        public ErrorCode CheckSolution(uint solution, Nonce clientNonce, Nonce serverNonce, uint puzzleDifficulty, uint clientIdentity)
        {
            if (puzzleDifficulty != CurrentDifficulty)
                return ErrorCode.InvalidPuzzleDifficulty;

            return CheckOneSolution(solution, clientNonce, serverNonce, puzzleDifficulty, clientIdentity) ? ErrorCode.Success : ErrorCode.InvalidSolution;
        }

        public static bool CheckOneSolution(uint solution, Nonce clientNonce, Nonce serverNonce, uint puzzleDifficulty, uint clientIdentity)
        {
            var buffer = new byte[24];

            var sol = BitConverter.GetBytes(solution);
            Array.Reverse(sol);

            var cid = BitConverter.GetBytes(clientIdentity);
            Array.Reverse(cid);

            Array.Copy(sol, 0, buffer, 0, 4);
            Array.Copy(cid, 0, buffer, 4, 4);
            Array.Copy(clientNonce.Data, 0, buffer, 8, 8);
            Array.Copy(serverNonce.Data, 0, buffer, 16, 8);

            var hash = new SHA256Managed().ComputeHash(buffer);

            var index = 0U;

            while (puzzleDifficulty > 8)
            {
                if (hash[index] != 0)
                    return false;

                ++index;
                puzzleDifficulty -= 8;
            }

            return (hash[index] & (0xFF << (8 - (int) puzzleDifficulty))) == 0;
        }

        public static bool SolvePuzzle(ref uint solution, Nonce clientNonce, Nonce serverNonce, uint puzzleDifficulty, uint clientIdentity)
        {
            var startTime = Environment.TickCount;
            var startValue = solution;

            while (true)
            {
                var nextValue = startValue + SolutionFragmentIterations;
                for (; startValue < nextValue; ++startValue)
                {
                    if (!CheckOneSolution(startValue, clientNonce, serverNonce, puzzleDifficulty, clientIdentity))
                        continue;

                    solution = startValue;
                    return true;
                }

                if (Environment.TickCount - startTime <= MaxSolutionComputeFragment)
                    continue;

                solution = startValue;
                return false;
            }
        }
    }
}
