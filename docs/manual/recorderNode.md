# Recorder Node

You can use the Recorder Node to quickly collect your data. To use the Recorder Node, first create a new Scripting graph.
> You can start with the recorderNode sample.

## Step 1: Create Data Container

You need to create a Saved Variable of an Aot Dictionary to store the data you need to record.

![](https://i.imgur.com/zLWJd8r.png)

As shown in the figure, we have created an Aot Dictionary named data, and within the dictionary, we have created the specific data we need to record.
> Supports all value types as well as UnityEngine’s Vector3 and Vector2.

## Step 2: Create Recorder

After the On Start event, add a Recorder Node.

![](https://i.imgur.com/cAxNljh.png)

As shown in the figure, the Recorder Node has four input ports:
- Data: This port receives a Dictionary as the data to be recorded. You need to pass the data variable created in the first step to this port.
- Name: You need to give your Recorder a name.
    > You can create multiple Recorder objects, and we use their names as their unique identifiers.
- Custom: You can add some extra descriptions for your Recorder.
- Prefix: You can customize the data storage location; the default storage location is Assets\Data.
    > While relative paths are supported, I still recommend using absolute paths if you are not familiar with Unity and the Windows file system.

![](https://i.imgur.com/GIitebg.png)

As shown in the figure, this is the basic usage of the Recorder Node. We use the Get Variable Node to obtain the data object we created in the first step and pass it to the Data port of the Recorder. Since this is our first Recorder object, we name it `hello`. For the Custom port, I assume there is a participant with the ID `001`. Finally, the files are stored in the default location (`Assets\Data`).

After this Recorder object is created, we print a completion message to the console.

When the Recorder object is created, it will create a file and path named `\<current date>(yyyy-mm-dd)\<current time>(hh-mm-ss)_RecorderName_Custom.csv` at the path you have set.

In this example, the following will be generated:

```
└─20240223
  └─114126_hello_001.csv
```

After the Recorder object creates the file, it automatically creates headers for each column of data. In this example, the result would look like this:

![](https://i.imgur.com/8VVyr3e.png)

## Step 3: Generate and Record Data

You can use Unity’s Dictionary Set Item Node to modify your data.

![](https://i.imgur.com/FphM0CV.png)

As shown in the figure, we modify the value of the key `speed` in the dictionary `data` to a random number between 0 and 10.

![](https://i.imgur.com/kF4qjod.png)

As shown in the figure, after modifying the data, we call the Recorder Write node to record the updated data to the file.

The Recorder Write Node has only one input port:
- Name: The name of the Recorder to perform the `write` operation on.

In this example, we modify our speed after the On Update event (which actually executes once per frame) and record the result of the modification to the file. 

The recorded results are as follows:

![](https://i.imgur.com/YPAzUzP.png)

We can see that each modification of the `speed` value has been recorded.

## Step 4: Stop the Recording

To avoid frequent opening and closing of files, thereby reducing IO consumption, the Recorder object does not actively close the file. When you no longer need to record data, you should manually call the Recorder Close Node to close the file.

![](https://i.imgur.com/BeFu3Jm.png)

As shown in the figure: We call the Recorder Close Node at the On Destroy event (in this example, the On Destroy event is triggered when the game ends) to close the Recorder object we created in the previous steps.

The Recorder Close Node, like the Recorder Write Node, has only one input port:
- The name of the Recorder to perform the `close` operation on.

## Summary

That completes the entire process of using the Recorder Node to collect and record data.

Finally, as shown in the figure below, we complete and organize this example.

![](https://i.imgur.com/S6kUFq4.png)

You can find this sample in [RecorderNode Sample](https://github.com/Ccccraz/PsychoUnity/blob/main/Assets/Plugins/PsychoUnity/Samples~/RecorderNode/Scripts/Recorder.asset)