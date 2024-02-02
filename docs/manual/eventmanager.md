# EventManger

```mermaid
sequenceDiagram
    Publisher->>EventManager: Add Event
    Subscriber_1->>EventManager: Listen Event
    Subscriber_2->>EventManager: Listen Event
    Subscriber_3->>EventManager: Listen Event
    Publisher->>EventManager: Trigger Event
    EventManager->>Subscriber_1: execute
    EventManager->>Subscriber_2: execute
    EventManager->>Subscriber_3: execute
```
