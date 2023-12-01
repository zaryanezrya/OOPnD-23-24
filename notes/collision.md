При перемещении объектов по игровому полю они могут сталкиваться между собой. 
Необходимо реализовать задачу столкновения игровых объектов. 
Реализация не должна зависеть от формы объектов, то есть код определения столкновения должен
удовлетворять OCP и отрабатывать за конечное время.

Указание. Применить деревья решений.

1.Реализовать команду, которая для двух игровых объектов определяет было столкновение или нет.

2. Предположим, что векторы признаков, с помощью которых определяется столкновение, 
записаны в файл. Необходимо прочитать векторы признаков и по ним построить деревья решений в памяти.
Доступ в деревьям решений должен выполняться через IoC.

```
[
    # Pre
    "CheckFuel"
    # Act
    "BurnFuel",
    "MoveCommand"
    # Post
    "CheckCollision"
]
```

Признаки:

```
x1, y1, dx1, dy1, x2, y2, dx2, dy2,

rx, ry, rdx, rdy
1   1   1    1 
1   1   1    0 

1   1  -1    0  
```

```
IoC.Resolve<ICommand>("Game.CheckCollision", UOb1, UOb2).Execute();

Execute(){
    //UOb1, UOb2
    {"rx": 1,
    ""}
    var features = IoC.Resolve<???>("Game.Collision.ExtractFeatures", UOb1, UOb2);
    IoC.Resolve<ICommand>("Game.Collision.Process", features).Execute();
}
```

https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.generic.collectionextensions.getvalueordefault?view=net-8.0


```
Action defaultAction = ()=> {

};
IDict<int, Action> prefix_tree;

Action act = prefix_tree.GetValueOrDefault(1, defaultAction);
act.Execute();

var leaf_action = (params object[] args) => {
    IoC.Resolve<ICommand>("Game.Process.Collision", args);
        // IoC.Resolve<ICommand>(Game.Object.Delete, obj1);
        // IoC.Resolve<ICommand>(Game.Object.Delete, obj2);
}

Func<object[], object>
(Func<object[], object>)object(args)
```
