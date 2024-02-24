# Timer Node

[Unity Visual Scripting](https://unity.com/features/unity-visual-scripting) provides a `Timer Node` implementation based on coroutines. In addition, we provide a `Timer Node` implementation based on asynchronous APIs. The following tutorials will introduce you to the use of the Timer Node step by step.

## Step 1: Create a Timer

You could use the `Timer Node` to create a `Timer` object.

![](https://i.imgur.com/EdT68KM.png)

As shown in the figure, the `Timer Node` has five input ports.
- **Target**: Limits the scope of `Timer`, with the default value being the `GameObject` itself
- **Event**: The name of the `Timer`, which will be the unique identifier for this `Timer`
- **Duration**: The duration of the `Timer`, in milliseconds
- **Delay**: The time to wait before the first timing, in milliseconds
- **Times**: The number of times the `Timer` needs to run
> When **Times** is set to ***-1***, the `Timer` will loop indefinitely

![](https://i.imgur.com/zMC7INA.png)

As shown in the figure, This is a named ***001*** with a duration of ***5000*** ms, a delay of ***0*** ms before the first timing, and a repetition of 5 times.

![](https://i.imgur.com/3SwqQy2.png)

As shown in the figure above, the `Timer` has the ability to pass parameters. You can control the number of parameters to be passed through the `Arguments` option at the header of the `Timer Node`, whit a maximum of ***10***.
> The parameters passed can be of any type

At this point, we have successfully to created a `Timer`

## Step 2: Listening Timer && Invoke Your Task

The implementation of the `Timer Node` relies on the ***Event*** mechanism of [Unity Visual Scripting](https://unity.com/features/unity-visual-scripting), so we only need to use the `Custom Event node`.

![](https://i.imgur.com/z2FKoqP.png)

As shown in the figure, we use `Custom Event Node` to listen to the `Timer` named ***001***. Since the `Timer` ***001*** we just created passes ***2*** parameters, we need to set the number of parameters to receive in the header of the `Custom Event Node` to ***2*** as well. Finally execute your task. In this example, we add the two parameters passed by `Timer` and print result to the console. This task will execute every ***5000*** ms for a total of ***5*** times.

The output result of this example is shown in the following figure:

![](https://i.imgur.com/87mh59s.png)

As you can see, the task execute every ***5*** s for a total of ***5*** times.

### Invoke MultiTask

A single Timer can trigger multiple tasks simultaneously.

![](https://i.imgur.com/UNZ2H1h.png)

As shown in the figure, we added the second task in this graph: we use the first parameter minus the second parameter passed by `Timer` and print result to console. The result is as follows:

![](https://i.imgur.com/yV2y7XY.png)

As you can see, both task were executed ***5*** times

## Summary

So far, we have complete the whole process of using `Timer Node` to control the delayed startup tasks and periodic tasks.

Finally, as shown in the following figure, we complete and organize this sample.

![](https://i.imgur.com/Ob4VzmZ.png)
