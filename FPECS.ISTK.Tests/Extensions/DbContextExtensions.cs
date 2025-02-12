using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace FPECS.ISTK.Tests.Extensions;
public static class DbContextExtensions
{
    /// <summary>
    /// Mocks an <see cref="EntityEntry"/>  for a given entity in the specified DbContext.
    /// <see href="https://stackoverflow.com/questions/79166875/how-to-mock-entityentry-in-ef-core">Source</see>
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="dbContextMock">The mock of the DbContext.</param>
    /// <param name="entity">The entity to mock the entry for.</param>
    /// <returns>A <see cref="Mock"> of the <see cref="EntityEntry"/> for the specified entity.</returns>
    /// <remarks>The method heavilly depends on "Internal EF Core API" and so can fail after EF upgrade. Use with extreme care.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    public static Mock<EntityEntry<TEntity>> MockEntityEntry<TContext, TEntity>(Mock<TContext> dbContextMock, TEntity entity)
    where TContext : DbContext
    where TEntity : class
    {
        var stateManagerMock = new Mock<IStateManager>();
        stateManagerMock
            .Setup(x => x.CreateEntityFinder(It.IsAny<IEntityType>()))
            .Returns(new Mock<IEntityFinder>().Object);
        stateManagerMock
            .Setup(x => x.ValueGenerationManager)
            .Returns(new Mock<IValueGenerationManager>().Object);
        stateManagerMock
            .Setup(x => x.InternalEntityEntryNotifier)
            .Returns(new Mock<IInternalEntityEntryNotifier>().Object);

        var entityTypeMock = new Mock<IRuntimeEntityType>();
        var keyMock = new Mock<IKey>();
        keyMock
            .Setup(x => x.Properties)
            .Returns([]);
        entityTypeMock
            .Setup(x => x.FindPrimaryKey())
            .Returns(keyMock.Object);
        entityTypeMock
            .Setup(e => e.EmptyShadowValuesFactory)
            .Returns(() => new Mock<ISnapshot>().Object);

        var internalEntityEntry = new InternalEntityEntry(stateManagerMock.Object, entityTypeMock.Object, entity);

        var entityEntryMock = new Mock<EntityEntry<TEntity>>(internalEntityEntry);
        dbContextMock
            .Setup(c => c.Entry(It.IsAny<TEntity>()))
            .Returns(() => entityEntryMock.Object);

        return entityEntryMock;
    }
}
