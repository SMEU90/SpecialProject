﻿using System.Threading.Tasks;

namespace SpecialProjectTest.Infrastructure
{
    internal delegate Task ActionAsync();

    internal delegate Task ActionAsync<in T>(T parameter);
}
