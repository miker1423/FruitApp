﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FruitAppAPI.Services.Interfaces
{
    public interface IFruitGraphService
    {
        Task CreateFruits(List<string> fruits);
        Task FindAndRelate(string nodeId, string fruitName);
        Task<IEnumerable<string>> GetFruits();
    }
}
