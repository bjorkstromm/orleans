﻿using TestExtensions;
using Xunit;

namespace TestServiceFabricMesh
{
    // Assembly collections must be defined once in each assembly

    [CollectionDefinition(TestEnvironmentFixture.DefaultCollection)]
    public class TestEnvironmentFixtureCollection : ICollectionFixture<TestEnvironmentFixture> { }
}