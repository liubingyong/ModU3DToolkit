using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PathologicalGames;
using DG.Tweening;
using System;

public class Reel : MonoBehaviour
{
    public enum SpinMode
    {
        Random,
        //Probability,
        Sequence,
    }

    public class SymbolWrapper
    {
        public int index;
        public string symbolName;

        public SymbolWrapper(int index, string symbolName)
        {
            this.index = index;
            this.symbolName = symbolName;
        }
    }

    //    [HideInInspector]
    public Symbol[] activeSymbols;
    public SpinMode spinMode;
    public float rowHeight = 1.5f;
    public int rowsCount = 1;
    public string[] initialSymbolNames;

    public float maxSpeed = 1f;
    public float acceleration = 0.5f;

    public float currentSpeed = 0f;

    [HideInInspector]
    public List<SymbolWrapper> allSymbols;
    [HideInInspector]
    public bool inSpin = false;
    [HideInInspector]
    public bool inDespin = false;
    [HideInInspector]
    public bool isRunning = false;

    private SlotMachine _slotMachine;

    public SlotMachine slotMachine
    {
        get
        {
            return _slotMachine;
        }
        set
        {
            _slotMachine = value;
            this.transform.SetParent(_slotMachine.transform);
        }
    }

    public string symbolType
    {
        get
        {
            return activeSymbols.Last().GetComponent<Symbol>().symbolType;
        }
    }

    [SerializeField]
    private UnityEvent _onFinished = new UnityEvent();

    public UnityEvent OnFinished
    {
        get { return _onFinished; }
        set { _onFinished = value; }
    }

    private SymbolWrapper lastElementPointer;

    void Start()
    {
        if (initialSymbolNames.Length < rowsCount)
        {
            return;
        }

        LoadSymbols(initialSymbolNames, initialSymbolNames.Length - 1);
    }

    public void Spin()
    {
        if (!isRunning)
        {
            isRunning = true;
            inSpin = true;
            currentSpeed = 0f;
        }
    }

    public void Despin()
    {
        inDespin = true;
    }

    public void Reset()
    {
        inSpin = false;
        isRunning = false;
        inDespin = false;

        currentSpeed = 0;

        allSymbols.Clear();
        activeSymbols = null;

        LoadSymbols(initialSymbolNames, initialSymbolNames.Length - 1);
    }

    public void ResetState()
    {
        inSpin = false;
        isRunning = false;
        inDespin = false;

        OnFinished.Invoke();
    }

    private List<Symbol> tlist = new List<Symbol>();
    private Vector3 tPosition = Vector3.zero;

    public void OnRolled(GameObject currentSymbol)
    {
        tlist.Clear();
        tPosition = Vector3.zero;

        var nextSymbolToSpawn = "";
        switch (spinMode)
        {
            case SpinMode.Random: nextSymbolToSpawn = PoolManager.Pools["Symbols"].prefabs.Keys.PickRandom(); break;
            //case SpinMode.Probability: break;
            case SpinMode.Sequence:
                lastElementPointer = allSymbols.NextOf(lastElementPointer);
                nextSymbolToSpawn = lastElementPointer.symbolName;
                break;
        }

        if (activeSymbols[rowsCount] != null)
        {
            tPosition = activeSymbols[rowsCount].transform.position;
            activeSymbols[rowsCount].OnRolled.RemoveAllListeners();
            PoolManager.Pools["Symbols"].Despawn(activeSymbols[rowsCount].transform);
        }
        activeSymbols[rowsCount] = PoolManager.Pools["Symbols"].Spawn(nextSymbolToSpawn).GetComponent<Symbol>();
        activeSymbols[rowsCount].reel = this;
        activeSymbols[rowsCount].OnRolled.AddListener(OnRolled);
        activeSymbols[rowsCount].transform.position = tPosition;
        activeSymbols[rowsCount].transform.localRotation = Quaternion.identity;

        tlist.Add(activeSymbols[rowsCount]);

        for (int i = 0; i < rowsCount; i++)
        {
            if (activeSymbols[i] != null)
            {
                tlist.Add(activeSymbols[i]);
            }
            else
            {
                tlist.Add(null);
            }
        }

        activeSymbols = tlist.ToArray();

        if (inDespin)
        {
            currentSpeed = 0f;

            for (int i = 0; i < activeSymbols.Length; i++)
            {
                activeSymbols[i].transform.DOLocalMoveY((0.5f - i) * rowHeight, 0.5f);

                if (i == activeSymbols.Length - 1)
                {
                    activeSymbols[i].transform.DOLocalMoveY((1 - i) * rowHeight, 0.2f).SetDelay(0.5f).OnComplete(() =>
                    {
                        ResetState();
                    });
                }
                else
                {
                    activeSymbols[i].transform.DOLocalMoveY((1 - i) * rowHeight, 0.2f).SetDelay(0.5f);
                }
            }

            return;
        }
    }

    void FixedUpdate()
    {
        if (inSpin && !inDespin)
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed = currentSpeed + acceleration * Time.fixedDeltaTime;
            }
        }
    }

    public void LoadSymbols(string[] symbols, int currentIndex = 0)
    {
        if (currentIndex >= symbols.Length)
        {
            throw new System.ArgumentOutOfRangeException();
        }

        if (activeSymbols == null || activeSymbols.Length == 0)
        {
            activeSymbols = new Symbol[rowsCount + 1];
        }

        allSymbols = new List<SymbolWrapper>();

        for (int i = 0; i < symbols.Length; i++)
        {
            var index = (currentIndex + i) % symbols.Length;
            allSymbols.Add(new SymbolWrapper(index, symbols[index]));
        }

        var tempSymbols = new SymbolWrapper[rowsCount + 1];
        for (int i = 0; i < tempSymbols.Length; i++)
        {
            if (i == 0)
            {
                tempSymbols[i] = allSymbols.First();
            }
            else
            {
                tempSymbols[i] = allSymbols.NextOf(tempSymbols[i - 1]);
            }

            var reversedIndex = tempSymbols.Length - 1 - i;
            activeSymbols[reversedIndex] = PoolManager.Pools["Symbols"].Spawn(tempSymbols[i].symbolName).GetComponent<Symbol>();
            activeSymbols[reversedIndex].reel = this;
            activeSymbols[reversedIndex].transform.localPosition = Vector3.zero;
            activeSymbols[reversedIndex].MoveTo(reversedIndex);
            activeSymbols[reversedIndex].transform.localRotation = Quaternion.identity;

            activeSymbols[reversedIndex].OnRolled.AddListener(OnRolled);
        }

        lastElementPointer = tempSymbols.Last();
    }
}