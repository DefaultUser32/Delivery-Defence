/* 1. description: handles logic for sending/managing waves
 * 2. @author: Linden/Matthew
 * 3. @date: 23-12-24
 * 4. @version: 1.0
 */
using UnityEngine;
using static WaveData;
using WaveEvent = WaveData.WaveEvent;
using EventType = WaveData.EventType;
using SendEvent = WaveData.SendEvent;
using SpecialEvent = WaveData.SpecialEvent;
using SpecialEventNames = WaveData.SpecialEventNames;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] TMP_Text roundCounter;
    [SerializeField] TextAsset[] levelData;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] PathParent[] paths;
    [SerializeField] GameObject enemyParent;

    public bool isWaveActive = false;
    public int waveNumber = -1;
    public int numberOfLoops = 0;
    public int activePath = 0;
    private Coroutine waveManager;

    // desc: starts the next wave
    // pre: if forceWaveStart, it will end the current wave, and start the next, otherwise, does nothing if wave is active
    public void StartNextWave(bool forceWaveStart)
    {
        if (!forceWaveStart && isWaveActive == true) {
            return;
        }

        if (waveManager != null)
        {
            StopCoroutine(waveManager);
            isWaveActive = false;
        }

        waveNumber++;

        if (waveNumber >= levelData.Length)
        {
            numberOfLoops++;
            waveNumber = -1;
            return;
        }

        StartWave(waveNumber, forceWaveStart);
    }

    // desc: force starts wave with given index
    // pre: int index of wave (MUST BE VALID WAVE), forceWaveStart with same logic as before
    // post: none
    public void StartWave(int waveIndex, bool forceWaveStart)
    {
        if (!forceWaveStart && isWaveActive == true) return;

        if (waveManager != null)
        {
            StopCoroutine(waveManager);
            isWaveActive = false;
        }

        waveNumber = waveIndex;
        activePath = 0;

        SetRoundCounter();
        waveManager = StartCoroutine(HandleWave(GetEvents(levelData[waveNumber].text)));
    }

    // desc: parses events from json
    // pre: valid waveData as json
    // post: data as a list of waveEvents
    WaveEvent[] GetEvents(string json)
    {
        return JsonUtility.FromJson<WaveData>(json).waveEvents;
    }

    // desc: handles all events given a list 
    // pre: valid list of WaveEvents
    // post: none (summons enemies but returns nothing)
    IEnumerator HandleWave(WaveEvent[] waveEvents)
    {
        isWaveActive = true;
        foreach (WaveEvent waveEvent in waveEvents)
        {
            switch (waveEvent.eventType)
            {
                case EventType.SEND:
                    for (int i = 0; i < waveEvent.sendEvent.quantity; i++)
                    {
                        CreateEnemy(enemyPrefabs[(int)waveEvent.sendEvent.sendType], activePath);
                        yield return new WaitForSeconds(waveEvent.sendEvent.spacing);
                    }
                    break;
                case EventType.WAIT:
                    yield return new WaitForSeconds(waveEvent.waitTime);
                    break;
                case EventType.SPECIAL:
                    HandleSpecial(waveEvent.specialEvent);
                    break;

            }
        }
        isWaveActive = false;
        yield return null;
    }

    // desc: handle logic for special events
    // pre: valid specialEvent
    // post: none
    void HandleSpecial(SpecialEvent specialEvent)
    {
        if (specialEvent.eventName == SpecialEventNames.CHANGE_PATH)
        {
            if (specialEvent.eventData >= paths.Length)
            {
                return;
            }
            activePath = specialEvent.eventData;
        }
        else if (specialEvent.eventName == SpecialEventNames.RANDOM_PATH)
        {
            activePath = Random.Range(0, paths.Length);
        }
    }


    // desc: summons a new enemy on a given path
    // pre: valid enemy prefab, valid index for the path
    // post: none
    void CreateEnemy(GameObject enemyPrefab, int pathIndex)
    {
        GameObject enemy = Instantiate(enemyPrefab);
        PathFollow p = enemy.GetComponent<PathFollow>();
        p.speed *= 1 + numberOfLoops;
        p.gameObject.GetComponent<BoxParent>().health *= 1.5f;
        p.path = paths[pathIndex];
        p.ResetPosition();
        enemy.transform.parent = enemyParent.transform;
    }

    // desc: updates round indicator
    // pre: none
    // post: none
    public void SetRoundCounter()
    {
        roundCounter.text = "Round " + GetRoundNumber();
    }

    // desc: gets the true round number from the waveNumber and number of loops
    // pre: none
    // post: round number as int
    public int GetRoundNumber()
    {
        return waveNumber + 1 + (numberOfLoops * levelData.Length);

    }
}
