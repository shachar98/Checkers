﻿using System.Collections.Generic;

namespace GameEnginesCommon
{
    public interface IGameState<Taction>
    {
        IEnumerable<Taction> GetNextActions(int agentIndex);

        IGameState<Taction> GetNextState(int agentIndex, Taction action);

        bool IsLost();

        bool IsWin();

        int NumOfAgents { get; }

        double CalcScore();
    }
}