/* 1. description: data class for representing one "wave" (round) of the game
 * 2. @author: Linden/Matthew
 * 3. @date: 23-12-24
 * 4. @version: 1.0
 */

// class for representing all data from 1 "wave" (round)
[System.Serializable]
public class WaveData
{
    // contains all things that happen
    public WaveEvent[] waveEvents;


    // data for 1 "event" (sending some enemies, changing the map, delay)
    [System.Serializable]
    public class WaveEvent {
        public EventType eventType;

        public SendEvent sendEvent; // null if not sending event
        public float waitTime; // non-0 only if waiting
        public SpecialEvent specialEvent; // null if not special event
    }

    // data for one batch of sending enemies
    [System.Serializable]
    public class SendEvent
    {
        public SendType sendType;
        public int quantity; 
        public float spacing;
    }

    // data for sending a "special" event
    [System.Serializable]
    public class SpecialEvent
    {
        public SpecialEventNames eventName;
        public int eventData;

    }

    public enum EventType : int
    {
        WAIT,
        SEND,
        SPECIAL
    }

    public enum SendType : int
    {
        CARDBOARD,
        WOOD,
        METAL
    }

    public enum SpecialEventNames : int
    {
        CHANGE_PATH,
        RANDOM_PATH
    }

}
