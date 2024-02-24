# Timer Node

[Unity Visual Scripting](https://unity.com/features/unity-visual-scripting) 提供一个基于协程的 `Timer` 实现, 作为补充我们提供一个基于异步 APIs 的 `Timer` 实现. 接下来的教程中会为你一步一步的介绍 `Timer Node` 的使用.

## Step 1: Create a Timer

你可以使用 `Timer Node` 来创建一个 `Timer` 对象

![](https://i.imgur.com/EdT68KM.png)

如图所示，`Timer Node ` 有五个输入端口:
- **Target**: 限制 `Timer` 的影响范围, 默认值为该 `GameObject` 自身
- **Event**: `Timer` 的名字, 这将是这个 `Timer` 的唯一标识
- **Duration**: `Timer` 的时间间隔, 单位为毫秒
- **Delay**: 在第一次记时前需要等待的时间, 单位为毫秒
- **Times**: `Timer` 需要循环运行的次数
> 当 **Times** 设置为 ***-1*** 时, 该 `Timer` 将会无限循环

![](https://i.imgur.com/zMC7INA.png)

如图所示, 这是一个名为 ***001*** Druation 为 ***5000*** ms 在第一次计时前等待 ***0*** ms 并重复 ***5*** 次的Timer

![](https://i.imgur.com/3SwqQy2.png)

如上图所示, `Timer Node` 有着传递参数的能力, 你可通过 `Timer Node` 头部的 `Argument` 选项来控制需要传递的参数数量, 上限为 `10` 个.
> 传递的参数可以是任何类型.

至此, 我们便成功创建了一个 `Timer`

## Step 2: Listening Timer && Invoke Your Task

`Timer Node` 的实现借助了 [Unity Visual Scripting](https://unity.com/features/unity-visual-scripting) 的 ***Event*** 机制, 所以我们只需要使用 `Custom Event` 节点即可.

![](https://i.imgur.com/z2FKoqP.png)

如图所示, 我们通过 `Custom Event Node` 来监听名为 ***001*** 的 `Timer` , 因为我的刚刚创建的 `Timer` ***001*** 传递 ***2*** 个参数, 所以我们需要在 `Custom Event Node` 的头部将接收的参数数量也设置为 ***2***, 然后执行你的任务. 在这个例子中, 我们将 `Timer` 所传递过来的两个参数相加并打印到控制台, 这个任务将会每间隔 ***5000*** ms 执行一次, 共执行 ***5*** 次.

本例的输出结果如下图所示:

![](https://i.imgur.com/87mh59s.png)

可以看到, 我的任务每隔 ***5*** s 运行了一次, 共运行了 ***5*** 次

### Invoke MultiTask

一个 `Timer` 可以同时 Trigger 多个任务

![](https://i.imgur.com/UNZ2H1h.png)

如图所示, 我们添加了第二个任务: 用 `Timer` 传递来的参数 ***0*** 减去 ***1*** 并打印, 运行结果如下:

![](https://i.imgur.com/yV2y7XY.png)

我们可以看到, 两个任务皆被执行了 ***5*** 次

## Summary

至此, 我们完成了使用 `Timer Node` 控制延时启动任务的和周期性任务的全流程.

最后, 如下图所示, 我们完成并整理这个例子.

![](https://i.imgur.com/Ob4VzmZ.png)
