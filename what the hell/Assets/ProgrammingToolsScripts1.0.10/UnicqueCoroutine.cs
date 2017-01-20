//versione: 1.0.1
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UniqueCoroutine : IEnumerator
{
    #region variables
    public class uniqueCoroutineDictionaryElement
    {
        public UniqueCoroutine _coroutine;
        public string tag;

        public uniqueCoroutineDictionaryElement() { }
        public uniqueCoroutineDictionaryElement(UniqueCoroutine cor)
        {
            _coroutine = cor;
            tag = "";
        }
        public uniqueCoroutineDictionaryElement(UniqueCoroutine cor, string tagline)
        {
            _coroutine = cor;
            tag = tagline;
        }

    }

    private static Dictionary<string, uniqueCoroutineDictionaryElement> _coroutines = new Dictionary<string, uniqueCoroutineDictionaryElement>();
    public bool stop;
    public bool _moveNext;
    string _name;
    IEnumerator enumerator;
    MonoBehaviour behaviour;
    public readonly Coroutine coroutine;
    #endregion

    #region constructor&Factory
    public UniqueCoroutine(MonoBehaviour behaviour, IEnumerator enumerator, string _refName)
    {
        this.behaviour = behaviour;
        this.enumerator = enumerator;
        this.stop = false;
        this._name = _refName;
        this.coroutine = this.behaviour.StartCoroutine(this);
    }

    public static void UCoroutine(MonoBehaviour behaviour, IEnumerator enumerator, string _name)
    {
        StopUCoroutine(_name);
        _coroutines.Add(_name,
                        new uniqueCoroutineDictionaryElement(new UniqueCoroutine(behaviour, enumerator, _name)
                                             )
                        );
    }

    public static void UCoroutine(MonoBehaviour behaviour, IEnumerator enumerator, string _name, string tag)
    {
        StopUCoroutine(_name);
        _coroutines.Add(_name,
                        new uniqueCoroutineDictionaryElement(new UniqueCoroutine(behaviour, enumerator, _name),
                                             tag
                                             )
                        );
    }
    #endregion

    #region IEnumerator Inherited
    public object Current { get { return enumerator.Current; } }

    public bool MoveNext()
    {
        _moveNext = enumerator.MoveNext();

        if (!_moveNext || stop)
        {
            return UniqueCoroutine.StopUCoroutine(_name);
        }

        else
            return _moveNext;
    }

    public void Reset() { enumerator.Reset(); }
    #endregion


    #region delayedExecution
    public delegate void delayedExecutable();
    public delegate void delayedExecutable<T>(T param);
    public delegate bool delayCondition();
    public delegate bool delayCondition<T>(T param);

    public static void executeInNextFrame(MonoBehaviour behaviour, delayedExecutable del)
    {
        behaviour.StartCoroutine(nextFrameExecutor(del));
    }

    static IEnumerator nextFrameExecutor(delayedExecutable del)
    {
        yield return new WaitForEndOfFrame();
        del();
    }

    public static void executeInNextFrame<T>(MonoBehaviour behaviour, delayedExecutable<T> del, T param)
    {
        behaviour.StartCoroutine(nextFrameExecutor(del, param));
    }

    static IEnumerator nextFrameExecutor<T>(delayedExecutable<T> del, T param)
    {
        yield return new WaitForEndOfFrame();
        del(param);
    }

    public static void executeAfterCondition<T, U>(MonoBehaviour behaviour, delayedExecutable<T> toExec, T execParam, delayCondition<U> cond, U condParam)
    {
        behaviour.StartCoroutine(afterConditionExecutor<T, U>(toExec, execParam, cond, condParam));
    }

    static IEnumerator afterConditionExecutor<T, U>(delayedExecutable<T> del, T param, delayCondition<U> cond, U condParam)
    {
        while (!cond(condParam))
        { yield return new WaitForEndOfFrame(); }
        del(param);
    }

    public static void executeAfterTime(MonoBehaviour behaviour, delayedExecutable toExec, float delay)
    {
        behaviour.StartCoroutine(afterTimeExecutor(toExec, delay));
    }

    static IEnumerator afterTimeExecutor(delayedExecutable toExec, float delay)
    {
        yield return new WaitForSeconds(delay);
        toExec();
    }
    /// <summary>
    /// repeatedly call 'do' every frame so long 'until' is true. wait for end of frame first, then 'do'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="behaviour"></param>
    /// <param name="toExec"></param>
    /// <param name="execParam"></param>
    /// <param name="cond"></param>
    /// <param name="condParam"></param>
    public static void doUntil(MonoBehaviour behaviour, delayedExecutable toDo, delayCondition until)
    {
        behaviour.StartCoroutine(doUntilExecutor(toDo, until));
    }
    static IEnumerator doUntilExecutor(delayedExecutable del, delayCondition cond)
    {
        while (cond())
        {
            yield return new WaitForEndOfFrame();
            del();
        }
    }

    public static void interruptibleDoUntil(MonoBehaviour behaviour, delayedExecutable toDo, delayCondition until, string name)
    {
        UCoroutine(behaviour, doUntilExecutor(toDo, until), name);
    }

    public static void pauseAfterTimeForeachDoUntil<T>(MonoBehaviour behaviour, delayedExecutable<T> toDo, ICollection<T> LoopArgument, float maxInterval, string name)
    {
        behaviour.StartCoroutine(pauseAfterTimeLoopDoUntilExecutor(toDo, LoopArgument, maxInterval));
    }
    static IEnumerator pauseAfterTimeLoopDoUntilExecutor<T>(delayedExecutable<T> del, ICollection<T> LoopArgument, float maxInterval)
    {
        float startingTime = Time.realtimeSinceStartup;

        foreach (T item in LoopArgument)
        {
            del(item);
            if (Time.realtimeSinceStartup > startingTime + maxInterval)
            {
                yield return new WaitForEndOfFrame();
                startingTime = Time.realtimeSinceStartup;
            }
        }
    }
    #endregion

    #region stoppers
    public static bool StopUCoroutine(string _name)
    {
        if (_coroutines.ContainsKey(_name))
        {
            _coroutines[_name]._coroutine.stop = true;
            _coroutines.Remove(_name);
            return true;
        }
        return false; //failed to stop coroutine because it doesn't exist
    }

    public static bool StopUCoroutineByTag(string _tag)
    {
        bool result = false;
        if (_coroutines.Count > 0)
        {
            //DebugConsole.Log("_coroutines.Count"+_coroutines.Keys.ToString());
            List<string> toBeRemoved = new List<string>();
            foreach (string item in _coroutines.Keys)
            {
                if (_coroutines[item].tag.Equals(_tag))
                {
                    _coroutines[item]._coroutine.stop = true;
                    toBeRemoved.Add(item);
                    result = true;
                }
            }
            foreach (var item in toBeRemoved)
            {
                _coroutines.Remove(item);
            }
        }
        return result; //failed to stop coroutine because no coroutine has such tag
    }
    #endregion
}