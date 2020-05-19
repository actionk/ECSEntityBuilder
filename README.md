# Unity ECS EntityBuilder

This project is a wrapper around Unity ECS entities that allows one to simplify the process of creating / modifying entities.

## Install

You can either just put the files into `Assets/Plugins/ECSEntityBuilder` or use it as a submodule:
```
git submodule add https://github.com/actionk/UnityECSEntityBuilder.git Assets/Plugins/ECSEntityBuilder
```

# Usage

## EntityWrapper

Entity Wrapper allows you to wrap `Entity` with an object a modify it without the need of passing the Entity reference to the EntityManager over and over again.

```
var entityWrapper = new EntityWrapper(entity)
    .SetComponentData(new Translation { Value = float3.zero });
    .AddBuffer<TaskBufferElement>();
    .AddBuffer(
        new PathPositionBufferElement {position = new int2(0, 1)},
        new PathPositionBufferElement {position = new int2(0, 2)}
    );
```

You can also specify target entity manager / command buffer when creating a wrapper:

```
EntityWrapper.Wrap(entity, PostUpdateCommands)
    .AddBuffer<MyBuffer>()
    .SetComponentData(new Translation {Value = command.position})
    .SetComponentData(new Scale {Value = 1.0f});
```

## EntityManagerWrapper

EntityManagerWrapper wraps around different ways of accessing EntityManager:
1. EntityManager
```
EntityManagerWrapper.Default
```

2. EntityCommandBuffer (PostUpdateCommands)
```
EntityManagerWrapper.FromCommandBuffer(PostUpdateCommands)
```

3. EntityCommandBuffer.Concurrent (Jobs)
```
EntityManagerWrapper.FromJobCommandBuffer(commandBuffer, threadId);
```

Usage example:
```
var entityWrapper = new EntityWrapper(entity);
entityWrapper.UsingWrapper(EntityManagerWrapper.FromCommandBuffer(PostUpdateCommands), wrapper =>
{
    wrapper.AddComponentData(new Rotation());
    wrapper.AddElementToBuffer(new PathPositionBufferElement {position = new int2(2, 0)});
});
```

Or using the builder:
```
var entityManagerWrapper = EntityManagerWrapper.FromJobCommandBuffer(commandBuffer, threadId);

UnitTaskBuilder
    .CreateCombined(itemPosition)
    .AddInnerTasks(
        UnitTaskBuilder.CreateInner(new PickUpItemTaskData {itemEntity = itemEntityToPickUp}).Build(entityManagerWrapper),
        UnitTaskBuilder.CreateInner(new PutItemToStorageTaskData {storageEntity = storageEntity}).Build(entityManagerWrapper)
    )
    .Build(entityManagerWrapper);
```

## EntityBuilder

```
public class ItemBuilder : EntityBuilder<ItemBuilder>
{
    public static ItemBuilder Create(ItemData itemData)
    {
        return new ItemBuilder(itemData);
    }

    protected override ItemBuilder Self => this;

    internal ItemBuilder(ItemData itemData)
    {
        var contentItem = DependencyProvider.Resolve<ContentItemRepository>().GetByKey(itemData.itemId);

        CreateFromArchetype<ArchetypeItem>();
        this.SetSpriteRenderer(contentItem.icon);
        SetVariable(new ZIndexVariable(SpriteLayers.OBJECT));
        SetComponentData(itemData);
        SetComponentData(new Scale {Value = 0.5f});
    }
}
```

Usage:
```
var itemEntity = ItemBuilder
    .Create(contentObject.gatheredResource)
    .SetPosition(EntityManager.GetComponentData<Translation>(taskData.objectEntity).Value)
    .Build(PostUpdateCommands);
```

You can pass the EntityManagerWrapper into the `Build()` function.

### Defining Archetypes

For defining an archetype you can just create a class implementing `IArchetypeDescriptor`:

```
public class ArchetypeItem : IArchetypeDescriptor
{
    public string Name => "Item";

    public ComponentType[] Components => new ComponentType[]
    {
        // common
        typeof(LocalToWorld),
        typeof(Translation),
        typeof(Rotation),
        typeof(Scale),

        // rendering
        typeof(MeshRendererData),

        // type-specific
        typeof(TagItem),
        typeof(ObjectData),
        typeof(ItemData)
    };
}
```

And then to create it inside the builder:
```
CreateFromArchetype<ArchetypeItem>();
```

### Initialising Archetypes

As you can only create archetypes in main thread by using EntityManager (not possible via EntityCommandBuffer), you can initialise your archetypes beforehand:

```
EntityArchetypeManager.Instance.InitializeArchetypes(
    // tasks
    typeof(ArchetypeCombinedUnitTask),
    typeof(ArchetypeSingleUnitTask),
    typeof(ArchetypeInnerUnitTask),

    // items
    typeof(ArchetypeItem),

    // zones
    typeof(ArchetypeZone),
    typeof(ArchetypeZoneCell),

    // objects
    typeof(ArchetypeDecoration),
    typeof(ArchetypeBuildingGround),

    // generic
    typeof(ArchetypeGenericObjectWithLocation)
);
```

Alternatively, you can initialize archetypes by marking those classes with [Archetype] attribute:

```
[Archetype]
public class ArchetypeZone : IArchetypeDescriptor
{
    public string Name => "Zone";

    public ComponentType[] Components => new ComponentType[]
    {
        ... components ...
    };
}
```

Then you can initialize archetypes for all the marked classes in this assembly with:

```
EntityArchetypeManager.Instance.InitializeArchetypes(Assembly.GetCallingAssembly());
```

### Initialising Archetypes in specific worlds

By default all the archetypes will be initialized in World.DefaultGameObjectInjectWorld
If you want to change this, you can specify the world by passing World parameter in the Attribute:

```
[Archetype(WorldType.SERVER)]
public class ServerPlayerArchetype : IArchetypeDescriptor
```

### Extending the EntityWrapper/EntityBuilder

You can easily add your own methods to EntityWrapper:

```
public static class EntityWrapper2DExtensions
{
    public static void SetPosition2D(this EntityWrapper wrapper, int2 position)
    {
        wrapper.SetComponentData(new Translation {Value = new float3(position.x + 0.5f, position.y + 0.5f, 0)});
    }
}
```

Or to EntityBuilder:

```
public static class EntityBuilder2DExtensions
{
    public static T SetPosition2D<T>(this EntityBuilder<T> wrapper, int2 position)
    {
        return wrapper.AddStep(new SetPosition2D(position));
    }
}

public class SetPosition2D : IEntityBuilderStep
{
    private readonly int2 position;

    public SetPosition2D(int2 position)
    {
        this.position = position;
    }

    public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
    {
        wrapper.SetComponentData(data.entity, new Translation {Value = new float3(position.x + 0.5f, position.y + 0.5f, 0)});
    }
}
```

### Additional functionality on Build()

To add your own Build implementation and access the EntityManager that was used for building, you can overload a `OnBuild` method:

```
public override void OnBuild(EntityManagerWrapper wrapper, Entity dataEntity)
{
    // your building code
}
```

### Variables

When extending the `EntityWrapper` you might want to save some data in the wrapper to reuse it in your extensions. For example, when setting the 2D position, you might want to save ZIndex to be used for this exact entity.

```
public class ZIndexVariable : IEntityVariable
{
    public int zIndex;

    public ZIndexVariable(int zIndex)
    {
        this.zIndex = zIndex;
    }
}
```

And then extend the EntityBuilder:

```
public class SetPosition2D : IEntityBuilderStep
{
    private const float ZIndexMultiplier = 0.01f;
    private readonly int2 position;

    public SetPosition2D(int2 position)
    {
        this.position = position;
    }

    private Translation GetTranslation(EntityVariableMap variables)
    {
        var zIndex = variables.Get<ZIndexVariable>()?.zIndex ?? 0;
        return new Translation {Value = new float3(position.x + 0.5f, position.y + 0.5f, -zIndex * ZIndexMultiplier)};
    }

    public void Process(EntityManagerWrapper wrapper, EntityVariableMap variables, ref EntityBuilderData data)
    {
        wrapper.SetComponentData(data.entity, GetTranslation(variables));
    }
}
```

Usage:

```
ObjectBuilder.Create(6)
    .SetVariable(new ZIndexVariable(10));
    .SetPosition2D(new int2 {x = -7, y = 0})
    .Build();
```
