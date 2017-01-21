//versione: 1.0.3
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;


public static class RandomHelper
{
    private static int seedCounter = new System.Random().Next();

    private static System.Random rng;

    public static System.Random Instance
    {
        get
        {
            if (rng == null)
            {
                int seed = Interlocked.Increment(ref seedCounter);
                rng = new System.Random(seed);
            }
            return rng;
        }
    }
}
public static class UtilityMods
{
    #region int
    //verifica se un int è dispari
    public static bool IsOdd(this int number)
    {
        return ((number % 2) != 0);
    }
    //verifica se un int è pari
    public static bool IsEven(this int number)
    {
        return ((number % 2) == 0);
    }
    #endregion

    #region transform
    /// <summary>
    /// uso: copia A su B ->A.CopyTo( B,false );
    /// </summary>
    /// <param name="toBeCloned">To be cloned.</param>
    /// <param name="toCloneTo">To clone to.</param>
    /// <param name="copyParent">If set to <c>true</c> copy parent.</param>
    public static void CopyTo(this Transform toBeCloned, Transform toCloneTo, bool copyParent)
    {

        toCloneTo.localPosition = toBeCloned.localPosition;
        toCloneTo.localRotation = toBeCloned.localRotation;
        toCloneTo.localScale = toBeCloned.localScale;


        if (copyParent)

            toCloneTo.parent = toBeCloned.parent;
    }
    /// <summary>
    /// azzera scala, rotazione e posizione LOCALI
    /// </summary>
    /// <param name="toBeCleared"></param>
	public static void Clear(this Transform toBeCleared)
    {
        toBeCleared.ResetRototraslation();
        toBeCleared.localScale = Vector3.one;
    }
    /// <summary>
    /// azzera rotazione e posizione LOCALI, ma non scala
    /// </summary>
    /// <param name="toBeCleared"></param>
	public static void ResetRototraslation(this Transform toBeCleared)
    {
        toBeCleared.localPosition = Vector3.zero;
        toBeCleared.localRotation = Quaternion.identity;
    }
    #endregion

    #region float
    //genera un float random tra min e max
    public static float initializeAtRandomMM(this float toInitialize, float min, float max)
    {
        toInitialize = ((float)RandomHelper.Instance.Next(0, 10001)) / 10000f;
        toInitialize = toInitialize * (max - min);
        toInitialize += min;
        return toInitialize;
    }
    /// <summary>
    /// restituisce un numero compreso tra 0 e 1 uguale a (toNormalize - min) / (max - min)
    /// operazione di normalizzazione dei dati. Sconsigliato l'uso nei cicli, meglio cachare il dividendo!
    /// </summary>
    /// <param name="toNormalize"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float normalized(this float toNormalize, float min, float max)
    {
        if ((max - min) == 0)
            Debug.LogError("float.normalized - divide by zero error prevented!");
        else
            return (toNormalize - min) / (max - min);
        return 1;
    }

    #endregion
    #region GUIextensions
    public static void SetUIobject(this RectTransform newTransform, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 leftTop, Vector2 rightBottom)
    {
        //imposta parent= scorepanel
        if (parent != null)
            newTransform.SetParent(parent);

        //applica posizioni anchor, totaldisplacement da l'offset tra i vari pannelli
        newTransform.anchorMin = anchorMin;
        newTransform.anchorMax = anchorMax;

        //imposta posizioni degli angoli
        newTransform.offsetMax = new Vector2(-rightBottom.x, -leftTop.y);
        newTransform.offsetMin = new Vector2(leftTop.x, rightBottom.y);
    }
    #endregion

    #region Vector extensions
    public static Vector3 getRandomHorizontal(this Vector3 toInitialize, float magnitude)
    {
        toInitialize = Quaternion.Euler(0, RandomHelper.Instance.Next(0, 359), 0) * (Vector3.right * magnitude);
        return toInitialize;
    }

    public static Vector2 meanPoint(this Vector2 a, Vector2 b)
    {
        return (a + ((b - a) / 2));
    }

    public static int manhattanDistance(this Vector2 a, Vector2 b)
    {
        //calcola la distanza di manhattan tra due vector2
        //ricorda che hai già sviluppato l'algoritmo per i path occupati nella 000
        return Mathf.Abs((int)(a.x - b.x)) + Mathf.Abs((int)(a.y - b.y));
    }

    #endregion
    #region LIST extensions


    /// <summary>
    /// mescola una lista (randomizzazione veloce ma statisticamente imperfetta)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        System.Random rng = RandomHelper.Instance;
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void PrintToLog<T>(this IList<T> list, string tag)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(tag + " [" + i + "](" + list[i].ToString() + ")");
        }

    }
    //	
    //	public static void LayeredPrintToDebugConsoleLog<T>(this IList<List<T>> list, string tag,bool toPrint, bool IsSummary)
    //	{
    //		if(toPrint)
    //		{
    //			if(IsSummary)
    //			{
    //				for (int i = 0; i < list.Count; i++) 
    //				{
    //					string summary=tag+"layer "+i+"";
    //					for (int j = 0; j < list[i].Count; j++) 
    //					{
    //						summary+="-"+list[i][j].ToString();
    //					}
    //					DebugConsole.Log(summary);
    //				}
    //			}
    //			else
    //			{
    //				for (int i = 0; i < list.Count; i++) 
    //				{
    //					list[i].PrintToDebugConsoleLog(tag+"-layer "+i+": ");
    //				}
    //			}
    //		}
    //	}
    //	
    //	public static void PrintToDebugConsoleLog<T>(this IList<T> list, string tag)  
    //	{  
    //		for (int i = 0; i < list.Count; i++) 
    //		{
    //			DebugConsole.Log(tag+" ["+i+"]("+list[i].ToString()+")");			
    //		}
    //		
    //	}
    //
    public delegate List<T> getNeighbours<T>(List<T> graph, T node, List<T> exploredNodes, Queue<T> enqueuedNodes);
    public static List<List<T>> getInBFSLayeredOrder<T>(this List<T> graph, T startingNode, getNeighbours<T> explore)
    {
        List<List<T>> result = new List<List<T>>();

        Queue<T> toExplore = new Queue<T>();
        List<T> startLayer = new List<T>();
        List<T> visitedNodes = new List<T>();
        startLayer.Add(startingNode);
        result.Add(startLayer);
        toExplore.Enqueue(startingNode);

        while (toExplore.Count > 0)
        {
            T exploring = toExplore.Peek();
            toExplore.Dequeue();
            visitedNodes.Add(exploring);

            List<T> newLayer = explore(graph, exploring, visitedNodes, toExplore);
            if (!result[result.Count - 1].Contains(exploring))
            {
                result.Add(newLayer);
            }
            else
            {
                result[result.Count - 1].AddRange(newLayer);
            }
        }


        return result;


    }
    #endregion

    #region ARRAY EXTENSIONS
    /// <summary>
    /// concatena x and y restituendo un array che ha entrambi
    /// </summary>
    /// <param name="x">The first array.</param>
    /// <param name="y">The second array.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T[] stick<T>(this T[] x, T[] y)
    {
        var z = new T[x.Length + y.Length];
        x.CopyTo(z, 0);
        y.CopyTo(z, x.Length);
        return z;
    }

    public static T[] getInner<T>(this T[,] x, int rowIndex)
    {
        T[] result = new T[x.GetLength(1)];
        for (int i = 0; i < x.GetLength(1); i++)
        {
            result[i] = x[rowIndex, i];
        }
        return result;
    }

    public static T[] getNthEachInner<T>(this T[,] x, int colIndex)
    {
        T[] result = new T[x.GetLength(0)];
        for (int i = 0; i < x.GetLength(0); i++)
        {
            result[i] = x[i, colIndex];
        }
        return result;
    }
    public static void initializeAtRandom01(this float[,] newarray)
    {
        System.Random rng = RandomHelper.Instance;

        for (int row = 0; row < newarray.GetLength(0); row++)
        {
            for (int col = 0; col < newarray.GetLength(1); col++)
            {
                newarray[row, col] = ((float)rng.Next(0, 10001)) / 10000f;
            }
        }
    }

    public static void initializeAtRandomMM(this float[,] newarray, float min, float max)
    {
        System.Random rng = RandomHelper.Instance;

        for (int row = 0; row < newarray.GetLength(0); row++)
        {
            for (int col = 0; col < newarray.GetLength(1); col++)
            {
                newarray[row, col] = ((float)rng.Next(0, 10001)) / 10000f;
                newarray[row, col] = newarray[row, col] * (max - min);
                newarray[row, col] += min;
            }
        }
    }

    public static bool Contains(this int[] array, int contained)
    {
        foreach (var item in array)
        {
            if (item == contained)
                return true;
        }
        return false;
    }

    public static void PrintToLog<T>(this T[] array, string tag, bool debugFlag)
    {
        if (debugFlag)
        {
            for (int i = 0; i < array.Length; i++)
            {

                Debug.Log(tag + " [" + i + "](" + array[i].ToString() + ")");
            }
        }

    }

    #endregion

    #region DICTIONARY EXTENSION
    public static void AddOrUpdate<T, U>(this IDictionary<T, U> dict, T key, U val)
    {
        if (dict.ContainsKey(key))
            dict[key] = val;
        else
            dict.Add(key, val);
    }
    public static void PrintToLog<T, U>(this IDictionary<T, U> dict, string tag)
    {
        foreach (var item in dict)
        {
            Debug.Log(tag + " (" + item.Key.ToString() + "," + item.Value.ToString() + ")");
        }
    }

    //	public static void PrintToDebugConsoleLog<T,U>(this IDictionary<T,U> dict, string tag)  
    //	{  
    //		foreach (var item in dict) {
    //			DebugConsole.Log(tag+" ("+item.Key.ToString()+","+item.Value.ToString()+")");
    //		}
    //	}

    #endregion

    public static T ToEnum<T>(this string enumString)
    {//usage: enumstring.ToEnum<TipoEnum>()
        return (T)System.Enum.Parse(typeof(T), enumString);
    }

    #region AnimationCurve

    public static float duration(this AnimationCurve curve)
    {
        if (curve.length == 0)
            return 0;

        return curve[curve.length - 1].time;
    }
    public static float lastValue(this AnimationCurve curve)
    {
        if (curve.length == 0)
            return 0;

        return curve[curve.length - 1].value;
    }
    #endregion

    //INIZIO REFLECTION

    public static void printObjectInLog<T>(this T obj, string tag, bool debugFlag)
    {

        if (debugFlag)
        {


            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {

                switch (property.PropertyType.ToString())
                {

                    case "System.Int32":

                        Debug.Log(tag + " - " + obj.GetType().GetProperty(property.Name));

                        break;
                    case "System.String":
                        Debug.Log(tag + " - " + obj.GetType().GetProperty(property.Name));

                        break;
                    case "System.List":

                        PropertyInfo propertyList = obj.GetType().GetProperty(property.Name);

                        IList<T> listaObj = propertyList.GetValue(obj, null) as IList<T>;

                        listaObj.PrintToLog(tag);

                        break;

                }
            }
        }

    }

    //FINE REFLECTION
}

