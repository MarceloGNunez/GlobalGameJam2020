﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class PipeManager : MonoBehaviour
{
    int currentLevelIndex = 0;

    public int gridSizeX, gridSizeY;

    public GameObject[] pipePrefab;
    public GameObject pipe_inicio;
    public GameObject pipe_fim;

    // Public LevelData[] levelsData <- UnityAction?

    public List<Vector2> randomPositions;

    float bufferInput = 0;
    float bufferInput_max = 0.2f;

    Vector2 selectedTile_pos = new Vector2();

    public GameObject selectedTile_selector;
    public GameObject selectedTile_marker;

    public GameObject[,] matriz;
    public GameObject pipesObjectsHolder;

    GameObject firstTileToChange;

    GameObject pipeFim_go;

    private Pipe currentPipe = null;

    [Header("Recursos")]
    public List<GameObject> recursos = new List<GameObject>();
    public List<GameObject> recursosPositions = new List<GameObject>();
    public GameObject recursosObjectsHolder;
    public Text madeiras_text;
    public Text fitas_text;
    public Text registros_text;

    [Header("Recursos 3D")]
    public List<GameObject> recursos3D = new List<GameObject>();
    public Transform positionToSpawn;

    [Header("Bordas da tela")]
    public Sprite bdTl_left_up;
    public Sprite bdTl_up_outer;
    public Sprite bdTl_up;
    public Sprite bdTl_up_right;
    public Sprite bdTl_up_right_outer;
    public Sprite bdTl_right;
    public Sprite bdTl_right_down;
    public Sprite bdTl_down;
    public Sprite bdTl_down_left;
    public Sprite bdTl_left;

    [Header("Items")]
    public int Madeiras = 0;
    public int Fitas = 0;
    public int Registros = 0;

    public LevelData[] levelDatas;

    public ModeManager modeManager;

    audioControl audioclips;
    AudioSource _audioSR;
    private void Awake()
    {
        audioclips = FindObjectOfType<audioControl>();
        _audioSR = audioclips.GetComponent<AudioSource>();
    }



    [HideInInspector]
    public bool canInteract = true;
    private Vector2 GetPositionFinish(Vector2 pos)
    {
        randomPositions.Remove(pos);
        return pos;
    }

    private Vector2 GetRandomPosition()
    {
        int randomIndex = (int)Random.Range(0, randomPositions.Count);
        Vector2 pos = randomPositions[randomIndex];
        randomPositions.Remove(pos);
        return pos;
    }



    private void Start()
    {
        canInteract = true;
        //LayOutLevel();
    }

    private void Update()
    {
        if (canInteract)
        {
            UpdateInteracting();
        }
    }

    private void UpdateInteracting()
    {
        if (bufferInput <= bufferInput_max)
            bufferInput += Time.deltaTime;

        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movement.sqrMagnitude > 0 && bufferInput > bufferInput_max)
        {
            Vector2 tempCheck = (selectedTile_pos + movement);
            if ((tempCheck.x < gridSizeX && tempCheck.x >= 0) && (tempCheck.y < gridSizeY && tempCheck.y >= 0))
            {
                bufferInput = 0;
                selectedTile_pos += movement;
                selectedTile_selector.transform.localPosition = selectedTile_pos * 100;
                _audioSR.PlayOneShot(audioclips.movingCursorTiles);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            var selectedGO = matriz[(int)selectedTile_selector.transform.localPosition.x / 100, (int)selectedTile_selector.transform.localPosition.y / 100];
            if (!selectedGO.name.Contains("pipe_empty") && !selectedGO.GetComponent<Pipe>().filledPipe)
            {
                _audioSR.PlayOneShot(audioclips.changeTiles);

                if (firstTileToChange == null)
                {
                    selectedTile_marker.transform.localPosition = selectedTile_selector.transform.localPosition;
                    firstTileToChange = selectedGO;
                }
                else
                {
                    var secondTileToChange = selectedGO;

                    matriz[(int)secondTileToChange.transform.localPosition.x / 100, (int)secondTileToChange.transform.localPosition.y / 100] = firstTileToChange;
                    matriz[(int)firstTileToChange.transform.localPosition.x / 100, (int)firstTileToChange.transform.localPosition.y / 100] = secondTileToChange;

                    var saveTileToChange = secondTileToChange.transform.localPosition;
                    secondTileToChange.transform.localPosition = firstTileToChange.transform.localPosition;
                    firstTileToChange.transform.localPosition = saveTileToChange;

                    selectedTile_marker.transform.localPosition = new Vector3(-1000, -1000, 0);
                    firstTileToChange = null;
                }
            }
        }
    }

    public void LayOutLevel(bool repeatLevel = true)
    {
        DestroyAll();

        LevelData levelData = levelDatas[currentLevelIndex];

        gridSizeX = levelData.levelSizeX;
        gridSizeY = levelData.levelSizeY;

        selectedTile_marker.transform.localPosition = new Vector3(-1000, -1000, 0);

        selectedTile_pos = new Vector2(0, gridSizeY - 1);
        selectedTile_selector.transform.localPosition = selectedTile_pos * 100;

        matriz = new GameObject[gridSizeX, gridSizeY];

        int objectsLeft = gridSizeX * gridSizeY;
        GameObject pipeObject;

        var posIni = new Vector3(-100, (gridSizeY - 1) * 100);
        var posFim = new Vector3(gridSizeX * 100, 0);

        GameObject pipePrefabSpawn = null;

        for (int i = 0; i < gridSizeX; ++i)
        {
            for (int j = 0; j < gridSizeY; ++j)
            {
                randomPositions.Add(new Vector2(i, j));
                var tempPos = new Vector3(i * 100, j * 100, 0);
                // Deveria estar lá embaixo
                if (true)
                {
                    if ((tempPos == posIni + new Vector3(+100, 0, 0)
                    || tempPos == posIni + new Vector3(+200, 0, 0)
                    || tempPos == posIni + new Vector3(+100, -100, 0)
                    || tempPos == posIni + new Vector3(+200, -100, 0))
                    || (tempPos == posFim + new Vector3(-100, 0, 0)
                    || tempPos == posFim + new Vector3(-200, 0, 0)
                    || tempPos == posFim + new Vector3(-100, +100, 0)
                    || tempPos == posFim + new Vector3(-200, +100, 0)
                    )
                    )
                    {
                        pipePrefabSpawn = pipePrefab[Random.Range(1, pipePrefab.Length)];
                    }
                    else
                    {
                        pipePrefabSpawn = pipePrefab[Random.Range(0, pipePrefab.Length)];
                    }

                    pipeObject = Instantiate(pipePrefabSpawn);
                    pipeObject.transform.SetParent(pipesObjectsHolder.transform);
                    pipeObject.transform.localPosition = tempPos;
                    matriz[i, j] = pipeObject;
                    pipeObject.GetComponent<Pipe>().Init(this);

                    if (!pipePrefabSpawn.name.Contains("empty") && Random.Range(0, 10f) >= 8f)
                    {
                        var recGo = Instantiate(recursos[Random.Range(0, recursos.Count)]);
                        recGo.GetComponent<Animator>().SetFloat("Offset", Random.Range(0.0f, 1.0f));
                        recGo.transform.SetParent(recursosObjectsHolder.transform);
                        recGo.transform.localPosition = tempPos;
                        recursosPositions.Add(recGo);
                    }
                }
            }
        }

        var pipeIni_go = Instantiate(pipe_inicio);
        pipeIni_go.transform.SetParent(pipesObjectsHolder.transform);
        pipeIni_go.transform.localPosition = new Vector3(-100, (gridSizeY - 1) * 100);

        pipeFim_go = Instantiate(pipe_fim);
        pipeFim_go.transform.SetParent(pipesObjectsHolder.transform);
        pipeFim_go.transform.localPosition = new Vector3(gridSizeX * 100, 0);
        pipeFim_go.GetComponent<Pipe>().Init(this);

        createBorders();

        currentPipe = pipeIni_go.GetComponent<Pipe>();
        Image fill = currentPipe.transform.GetChild(1).GetChild(0).GetComponent<Image>();

        selectedTile_selector.transform.localPosition = matriz[0, gridSizeY - 1].transform.localPosition;

        fill.DOFillAmount(1, levelData.initialTimer).SetEase(Ease.Linear).OnComplete(
            () =>
            {
                CallEnterWater(Pipe.PipeDirections.Left);
            }
        );

    }

    void createBorders()
    {

        for (int i = 0; i < gridSizeY; ++i)
        {

            if (new Vector3(-100, i * 100, 0) != new Vector3(-100, (gridSizeY - 1) * 100)
                && new Vector3(-100, i * 100, 0) != new Vector3(-100, (gridSizeY - 2) * 100))
                setBorda(bdTl_left, new Vector3(-100, i * 100, 0));

            if (new Vector3(gridSizeY * 100, i * 100, 0) != new Vector3(gridSizeX * 100, 0)
                && new Vector3(gridSizeY * 100, i * 100, 0) != new Vector3(gridSizeX * 100, 100))
                setBorda(bdTl_right, new Vector3(gridSizeY * 100, i * 100, 0));
        }

        for (int i = 0; i < gridSizeX; ++i)
        {
            setBorda(bdTl_up, new Vector3(i * 100, gridSizeY * 100, 0));
            setBorda(bdTl_down, new Vector3(i * 100, -100, 0));
        }

        var posIni = new Vector3(-100, (gridSizeY - 1) * 100);
        setBorda(bdTl_up, posIni + new Vector3(0, 100, 0));
        setBorda(bdTl_left_up, posIni + new Vector3(-100, 100, 0));
        setBorda(bdTl_left, posIni + new Vector3(-100, 0, 0));
        setBorda(bdTl_down_left, posIni + new Vector3(-100, -100, 0));
        setBorda(bdTl_up_right_outer, posIni + new Vector3(0, -100, 0));
        setBorda(bdTl_up_right, new Vector3((gridSizeX * 100), (gridSizeY * 100), 0));
        setBorda(bdTl_down_left, new Vector3(-100, -100, 0));

        var posFim = new Vector3(gridSizeX * 100, 0);
        setBorda(bdTl_down, posFim + new Vector3(0, -100, 0));
        setBorda(bdTl_right_down, posFim + new Vector3(+100, -100, 0));
        setBorda(bdTl_right, posFim + new Vector3(+100, 0, 0));
        setBorda(bdTl_up_right, posFim + new Vector3(+100, +100, 0));
        setBorda(bdTl_up_outer, posFim + new Vector3(0, +100, 0));


    }

    void setBorda(Sprite sprite, Vector3 pos)
    {
        var borda = new GameObject().AddComponent(typeof(Image));
        borda.GetComponent<Image>().sprite = sprite;
        borda.transform.SetParent(pipesObjectsHolder.transform);
        borda.transform.localPosition = pos;
    }

    public void CallEnterWater(Pipe.PipeDirections enterDirection)
    {
        Vector2 nextPipe = currentPipe.transform.localPosition / 100;
        switch (enterDirection)
        {
            case Pipe.PipeDirections.Left:
                nextPipe += new Vector2(1, 0);
                break;
            case Pipe.PipeDirections.Right:
                nextPipe += new Vector2(-1, 0);
                break;
            case Pipe.PipeDirections.Down:
                nextPipe += new Vector2(0, 1);
                break;
            case Pipe.PipeDirections.Up:
                nextPipe += new Vector2(0, -1);
                break;
        }


        if ((nextPipe.x < 0 || nextPipe.x >= gridSizeX || nextPipe.y < 0 || nextPipe.y >= gridSizeY))
        {
            if (nextPipe.x == gridSizeX && nextPipe.y == 0)
            {
                //Chegou no tile final
                currentPipe = pipeFim_go.GetComponent<Pipe>();
                currentPipe.GetComponent<Pipe>().EnterWater(enterDirection);
            }
            else
            {
                // Next tile is outside the grid
                // Troca do jogo 2D para 3D
                print("Este cano dá para fora do grid. GAME Over");
                modeManager.Swap("3d");
            }
        }
        else
        {
            // Next tile is inside the grid
            currentPipe = matriz[(int)nextPipe.x, (int)nextPipe.y].GetComponent<Pipe>();
            currentPipe.GetComponent<Pipe>().EnterWater(enterDirection);
        }
    }

    public void DestroyAll()
    {

        foreach (Transform child in transform.GetChild(0).GetChild(0))
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in transform.GetChild(0).GetChild(1))
        {
            Destroy(child.gameObject);
        }
        recursosPositions.Clear();

        selectedTile_marker.transform.localPosition = new Vector3(-1000, -1000, 0);
        selectedTile_selector.transform.localPosition = new Vector3(-1000, -1000, 0);

        //LayOutLevel();
    }

    public void AddMadeiras()
    {
        Madeiras++;
        madeiras_text.text = "x" + Madeiras;
    }
    public void AddFitas()
    {
        Fitas++;
        fitas_text.text = "x" + Fitas;
    }
    public void AddRegistros()
    {
        Registros++;
        registros_text.text = "x" + Registros;
    }
}
