[<- Return to README](../README.md)

# Destination Graph

The `DestinationGraph` is the core part of the Commuter Destination mod. It represents, from the perspective of a given "origin" stop, the set of all intended journeys for all passengers currently waiting at that stop.

The `DestinationGraphGenerator` iterates through all passengers, and identifies which stop they are intending to travel to, from the current stop. It then looks up (or creates) a `DestinationGraphStop` for that stop. Next, it looks at the building that the passenger will travel to after they reach their destination stop, and adds a `DestinationGraphJourney` to the `DestinationGraphStop` for that building. If there is already a `DestinationGraphJourney` for that building, it increments the count of passengers for that journey.

![image showing a `DestinationGraph` containing one origin stop and two `DestinationGraphStop`s. The stops have a number of `DestinationGraphJourney`s each](docs/graph.png)

It is important to remember that the `DestinationGraph` does not encompass the whole transport line, but only contains the journeys to be taken by the passengers currently waiting at the origin stop.