﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Cysharp.Threading.Tasks;
/// <summary>
/// オブジェクトのセーブ、ロードを一括で行える管理クラス
/// </summary>
sealed public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
{
    [SerializeField] bool loadOnStart = false;
    private void Start()
    {
        if (loadOnStart)
        {
            Load();
            LoadAsync();
        }
    }
    Queue<ISaveable> m_saveables = new Queue<ISaveable>();
    Queue<ILoadable> m_loadables = new Queue<ILoadable>();
    Queue<ISaveableAsync> m_saveablesAsync = new Queue<ISaveableAsync>();
    Queue<ILoadableAsync> m_loadablesAsync = new Queue<ILoadableAsync>();
   public UnityEvent OnLoadFinished { get; } = new UnityEvent();
   public UnityEvent OnSaveFinished { get; } = new UnityEvent();
    public enum SaveLoadState
    {
        notLoaded,
        loading,
        finished
    }

    public SaveLoadState LoadState { get; private set; } = SaveLoadState.notLoaded;
    public SaveLoadState SaveState { get; private set; } = SaveLoadState.notLoaded;

    /// <summary>
    /// GamaManager.Start()でデータがロードされるので、Awake内で行ってください。
    /// </summary>
    /// <param name="item"></param>
    public void SetLoadable(ILoadable item) { m_loadables.Enqueue(item); }

    /// <summary>
    /// GamaManager.Start()でデータがロードされるので、Awake内で行ってください。
    /// </summary>
    /// <param name="item"></param>
    public void SetLoadable(ILoadableAsync item) { m_loadablesAsync.Enqueue(item); }

    /// <summary>
    /// GamaManager.Start()でデータがロードされるので、Awake内で行ってください。
    /// </summary>
    /// <param name="item"></param>
    public void SetSaveable(ISaveable item) { m_saveables.Enqueue(item); }

    /// <summary>
    /// GamaManager.Start()でデータがロードされるので、Awake内で行ってください。
    /// </summary>
    /// <param name="item"></param>
    public void SetSaveable(ISaveableAsync item) { m_saveablesAsync.Enqueue(item); }
    public void Save()
    {
        while (m_saveables.Count > 0)
        {
            var obj = m_saveables.Peek();
            obj.Save();
            m_saveables.Dequeue();
        }
    }
    /// <summary>
    /// セーブする関数。
    /// StartもしくはUpdate内で呼んでください。
    /// Awake、OnEnableで呼ぶとSaveできないオブジェクトが発生する可能性があります。
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        SaveState = SaveLoadState.loading;
        while (m_saveablesAsync.Count > 0)
        {
            var obj = m_saveablesAsync.Peek();
            await obj.SaveAsync(saveCancellationTokenSource.Token);
            m_saveablesAsync.Dequeue();
        }
        //非同期でセーブし、すべてのオブジェクトについて完了するまで待つ
        Debug.Log("All Data Saving Finished");
        SaveState = SaveLoadState.finished;
        OnSaveFinished.Invoke();
    }
    public void Load()
    {
        while (m_loadables.Count > 0)
        {
            var obj = m_loadables.Peek();
            obj.Load();
            m_loadables.Dequeue();
        }
    }
    /// <summary>
    /// ロードする関数。
    /// StartもしくはUpdate内で呼んでください。
    /// Awake、OnEnableで呼ぶとLoadできないオブジェクトが発生する可能性があります。
    /// </summary>
    /// <returns></returns>
    public async Task LoadAsync()
    {
        LoadState = SaveLoadState.loading;
        while (m_loadablesAsync.Count > 0)
        {
            var obj = m_loadablesAsync.Peek();
            await obj.LoadAsync(loadCancellationTokenSource.Token);
            m_loadablesAsync.Dequeue();
        }
        //非同期でロードし、すべてのオブジェクトについて完了するまで待つ
        Debug.Log("All Data Loading Finished");
        LoadState = SaveLoadState.finished;
        OnLoadFinished.Invoke();
    }


    private CancellationTokenSource loadCancellationTokenSource;
    private CancellationTokenSource saveCancellationTokenSource;
    private void Awake()
    {
        loadCancellationTokenSource = new CancellationTokenSource();
        saveCancellationTokenSource = new CancellationTokenSource();
    }

}
