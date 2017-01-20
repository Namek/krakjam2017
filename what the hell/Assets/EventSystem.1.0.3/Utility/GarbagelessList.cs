using System;
using System.Collections.Generic;

public class GarbagelessList
{
    #region enumerator
    bool enumeratorJustReset = false;
    int currentIndex;
    public gameEventHandler Current
    {
        get
        {
            return getContentAtPos(currentIndex);
        }
    }


    public bool MoveNext()
    {
        if (Count == 0)
            return false;
        if (enumeratorJustReset)
        {
            enumeratorJustReset = false;
            return true;
        }
        if (currentIndex == tail)
            return false;
        currentIndex = getNextIndexAtPos(currentIndex);
        return true;
    }

    public void Reset()
    {
        currentIndex = head;
        enumeratorJustReset = true;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
    #endregion


    const int DEFAULTCAPACITY = 1000;
    int capacity;
    public int Count { get; private set; }
    int head;
    int tail;
    int freeHead;
    int freeTail;
    node[][] ArrayData;
    int newIndexCache;

    struct node
    {
        public gameEventHandler content;
        public int nextIndex;//default is 0
        public int lastIndex;//default is 0
        public bool isUsed;//default is false
    }
    BST bstIndex;


    #region public functions
    public GarbagelessList()
    {
        ArrayData = new node[DEFAULTCAPACITY][];
        bstIndex = new BST();
        addBatch();
        head = -1;
        tail = -1;
        freeHead = 0;
        Count = 0;
    }

    public void Add(gameEventHandler toAdd)
    {
        //only on the first add we use an ad hoc setup
        if (head == -1)
        {
            head = freeHead;
            tail = freeHead;
            freeHead = getNextIndexAtPos(freeHead);
            setContentAtPos(head, toAdd);
            setIsUsedAtPos(head, true);
            bstIndex.Add(head, toAdd.GetHashCode());
        }
        else
        {
            newIndexCache = getFree();
            freeHead = getNextIndexAtPos(freeHead);
            setNextIndexPos(tail, newIndexCache);
            setLastIndexAtPos(newIndexCache, tail);
            tail = newIndexCache;
            setIsUsedAtPos(newIndexCache, true);
            setContentAtPos(newIndexCache, toAdd);
            bstIndex.Add(newIndexCache, toAdd.GetHashCode());
        }
        ++Count;
    }

    public void Remove(gameEventHandler toRemove)
    {
        int i = bstIndex.getIndexList(toRemove.GetHashCode())[0];

        if (getIsUsedAtPos(i))
            if (getContentAtPos(i) == toRemove)
            {
                if (head != i)
                    setNextIndexPos(getLastIndexAtPos(i), getNextIndexAtPos(i));
                else
                { head = getNextIndexAtPos(i); }
                if (tail != i)
                    setLastIndexAtPos(getNextIndexAtPos(i), getLastIndexAtPos(i));
                else
                { tail = getLastIndexAtPos(i); }

                setIsUsedAtPos(i, false);
                setLastIndexAtPos(freeHead, i);
                setNextIndexPos(i, freeHead);
                freeHead = i;

                --Count;
            }
    }
    /*
    public gameEventHandler this[int key]
    {
        get
        {
            return getContentAtPos(key);
        }
        set
        {
            setContentAtPos(key, value);
        }
    }
    */
    #endregion
    #region arrayWrap
    gameEventHandler getContentAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].content;
    }
    void setContentAtPos(int key, gameEventHandler value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].content = value;
    }

    int getNextIndexAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].nextIndex;
    }
    void setNextIndexPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].nextIndex = value;
    }

    int getLastIndexAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].lastIndex;
    }
    void setLastIndexAtPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].lastIndex = value;
    }

    bool getIsUsedAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].isUsed;
    }
    void setIsUsedAtPos(int key, bool value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].isUsed = value;
    }
    #endregion

    #region memory management
    int getFree()
    {
        //if arraydata is full, we need to expand it.
        if (Count >= capacity)
            addBatch();
        int result = freeHead;
        freeHead = getNextIndexAtPos(freeHead);
        return result;
    }

    void addBatch()
    {
        if (capacity > 0)
            UnityEngine.Debug.LogError("Had to extend GarbagelessList, set a higher DEFAULTCAPACITY for GarbagelessList or get garbage");
        ArrayData[capacity / DEFAULTCAPACITY] = new node[DEFAULTCAPACITY];

        for (int i = capacity; i < capacity + DEFAULTCAPACITY; i++)
        {
            setNextIndexPos(i, i + 1);
            setLastIndexAtPos(i, i - 1);
        }
        capacity += DEFAULTCAPACITY;
        freeTail = capacity - 1;

    }
    #endregion

}


public class BST
{
    indexerCouple[][] ArrayData;
    const int DEFAULTCAPACITY = 1000;
    public BST()
    {

        ArrayData = new indexerCouple[DEFAULTCAPACITY][];
        indexlistHead = -1;
        poolHead = -1;
        capacity = 0;
        count = 0;
        addBatch();
    }

    #region indexercoupledef

    struct indexerCouple
    {
        public int[] indexedArrayData;//-1=free slot
        public int hashValue;
        public int leftChildIndex;//default is 0
        public int rightChildIndex;//default is 0
        public int lastIndex;//default is 0
        public bool isRed;

        public int ContentCount;

        public void resize()
        {
            int oldSize = indexedArrayData.Length;
            Array.Resize<int>(ref indexedArrayData, indexedArrayData.Length * 2);
            for (int i = oldSize; i < indexedArrayData.Length; i++)
            {
                indexedArrayData[i] = -1;
            }
            UnityEngine.Debug.LogError("Had to extend indexedArrayData, set a higher DEFAULTCAPACITY for BST indexer indexedArrayData or get garbage");
        }
    }

    public bool blackOrNull(int toCeck) { return (toCeck == -1) ? true : (!getIsRedAtPos(toCeck)); }

    public int Sibling(int index)
    {
        int lastIndex = getLastIndexAtPos(index);
        if (lastIndex == -1)
            return -1;
        if (index == getLeftChildIndexAtPos(lastIndex))
            return getRightChildIndexAtPos(lastIndex);
        else
            return getLeftChildIndexAtPos(lastIndex);
    }
    public bool isInternal(int index)
    {
        int lastIndex = getLastIndexAtPos(index);
        if (lastIndex == -1)
            return false;
        if (getLastIndexAtPos(lastIndex) == -1)
            return false;
        if (getRightChildIndexAtPos(lastIndex) == index && getLeftChildIndexAtPos(getLastIndexAtPos(lastIndex)) == lastIndex)
            return true;

        if (getLeftChildIndexAtPos(lastIndex) == index && getRightChildIndexAtPos(getLastIndexAtPos(lastIndex)) == lastIndex)
            return true;
        return false;
    }

    public int InternalChild(int index)
    {
        int lastIndex = getLastIndexAtPos(index);
        if (lastIndex == -1) return -1;
        if (getRightChildIndexAtPos(lastIndex) == index)
            return getLeftChildIndexAtPos(index);
        else
            return getRightChildIndexAtPos(index);

    }

    public int ExternalChild(int index)
    {
        int lastIndex = getLastIndexAtPos(index);
        if (lastIndex == -1) return -1;
        if (getRightChildIndexAtPos(lastIndex) == index)
            return getRightChildIndexAtPos(index);
        else
            return getLeftChildIndexAtPos(index);
    }


    public bool indexerContains(int index, int value)
    {
        for (int i = 0; i < getIndexedArrayDataAtPos(index).Length; i++)
        {
            if (getIndexedArrayDataAtPos(index, i) == value)
                return true;
        }
        return false;
    }
    public void indexerAdd(int index, int value)
    {
        for (int i = 0; i < getIndexedArrayDataAtPos(index).Length; i++)
        {
            if (getIndexedArrayDataAtPos(index, i) == -1)
            {
                setIndexedArrayDataIndexPos(index, i, value);
                incrementIndexedDataCountAtPos(index);
                return;
            }
        }
        resizeIndexedDataCountAtPos(index);
        indexerAdd(index, value);
    }
    public void indexerRemove(int index, int value)
    {
        for (int i = 0; i < getIndexedArrayDataAtPos(index).Length; i++)
        {
            if (getIndexedArrayDataAtPos(index, i) == value)
            {
                setIndexedArrayDataIndexPos(index, i, -1);
                decrementIndexedDataCountAtPos(index);
                return;
            }
        }
    }
    #endregion

    int indexlistHead;
    int poolHead;
    int capacity;
    int count;
    #region query
    public int[] getIndexList(int hashvalue)
    {
        int result = findHash(hashvalue, indexlistHead);
        if (result == -1)
            throw new KeyNotFoundException();
        else
            return getIndexedArrayDataAtPos(result);
    }
    int findHash(int hashvalue, int currentNode)
    {
        if (currentNode == -1) return -1;
        if (getHashAtPos(currentNode) == hashvalue) return currentNode;
        return findHash(hashvalue, (getHashAtPos(currentNode) > hashvalue) ? getLeftChildIndexAtPos(currentNode) : getRightChildIndexAtPos(currentNode));
    }
    int findHashParent(int hashvalue, int currentNode, int parentNode)
    {
        if (currentNode == -1) return parentNode;
        return findHashParent(hashvalue, (getHashAtPos(currentNode) > hashvalue) ? getLeftChildIndexAtPos(currentNode) : getRightChildIndexAtPos(currentNode), currentNode);
    }

    #endregion

    #region pool Management
    void addBatch()
    {
        if (capacity > 0)
            UnityEngine.Debug.LogError("Had to extend BST, set a higher DEFAULTCAPACITY for BST or get garbage");

        ArrayData[capacity / DEFAULTCAPACITY] = new indexerCouple[DEFAULTCAPACITY];

        poolHead = capacity;
        for (int i = capacity; i < capacity + DEFAULTCAPACITY; i++)
        {
            setLastIndexAtPos(i, i + 1);
            setHashIndexPos(i, -1);
            setLeftChildIndexAtPos(i, -1);
            setRightChildIndexAtPos(i, -1);
            int[] toBeSet = new int[DEFAULTCAPACITY];
            for (int j = 0; j < DEFAULTCAPACITY; j++)
            {
                toBeSet[j] = -1;
            }
            setIndexedArrayDataAtPos(i, toBeSet);
        }
        capacity += DEFAULTCAPACITY;

    }

    int makeNew()
    {
        if (poolHead < capacity)
        {
            int result = poolHead;
            poolHead = getLastIndexAtPos(poolHead);
            setLastIndexAtPos(result, -1);
            resetIndexedDataCountAtPos(result);
            return result;
        }
        else
        {
            addBatch();
            return makeNew();
        }
    }
    void dispose(int toDispose)
    {
        setLastIndexAtPos(toDispose, poolHead);
        poolHead = toDispose;
    }
    #endregion
    #region rotation
    void rotateLeft(int parent)
    {
        //x= parent, y=rightchild
        int rightChildIndex = getRightChildIndexAtPos(parent);
        //move rightchild to his new parent
        if (parent != indexlistHead)
        {
            if (getLeftChildIndexAtPos(getLastIndexAtPos(parent)) == parent)
                setLeftChildIndexAtPos(getLastIndexAtPos(parent), rightChildIndex);
            else
                setRightChildIndexAtPos(getLastIndexAtPos(parent), rightChildIndex);
        }
        else
        {
            indexlistHead = rightChildIndex;
        }
        setLastIndexAtPos(rightChildIndex, getLastIndexAtPos(parent));

        int B = getLeftChildIndexAtPos(rightChildIndex);

        //move parent as the new rightchild's leftchild
        setLeftChildIndexAtPos(rightChildIndex, parent);
        setLastIndexAtPos(parent, rightChildIndex);

        //move B to his new parent
        setRightChildIndexAtPos(parent, B);
        if (B != -1)
            setLastIndexAtPos(B, parent);

    }

    void rotateRight(int parent)
    {
        //x= parent, y=leftchild
        int leftChildIndex = getLeftChildIndexAtPos(parent);

        //move leftChild to his new parent
        if (parent != indexlistHead)
        {
            if (getLeftChildIndexAtPos(getLastIndexAtPos(parent)) == parent)
                setLeftChildIndexAtPos(getLastIndexAtPos(parent), leftChildIndex);
            else
                setRightChildIndexAtPos(getLastIndexAtPos(parent), leftChildIndex);
        }
        else
        {
            indexlistHead = leftChildIndex;
        }
        setLastIndexAtPos(leftChildIndex, getLastIndexAtPos(parent));

        int B = getRightChildIndexAtPos(leftChildIndex);

        //move parent as the new leftchild's rightchild
        setRightChildIndexAtPos(leftChildIndex, parent);
        setLastIndexAtPos(parent, leftChildIndex);

        //move B to his new parent
        setLeftChildIndexAtPos(parent, B);
        if (B != -1)
            setLastIndexAtPos(B, parent);

    }
    /// <summary>
    /// revert child-parent relationship through rotation
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    void rotationSwap(int parent, int child)
    {
        if (getLeftChildIndexAtPos(parent) == child)
            rotateRight(parent);
        else rotateLeft(parent);
    }
    #endregion
    #region insertion
    public void Add(int indexedArrayElement, int hashvalue)
    {
        if (indexlistHead == -1)
        {
            indexlistHead = makeNew();
            setHashIndexPos(indexlistHead, hashvalue);

            indexerAdd(indexlistHead, indexedArrayElement);
            setIsRedAtPos(indexlistHead, false);

        }
        else
        {
            int holder = findHash(hashvalue, indexlistHead);
            if (holder != -1)
            {
                if (!indexerContains(holder, indexedArrayElement))
                {
                    indexerAdd(holder, indexedArrayElement);
                }
            }
            else
            {
                holder = makeNew();
                setHashIndexPos(holder, hashvalue);
                indexerAdd(holder, indexedArrayElement);
                redBlackInsertion(holder);
            }
        }
        ++count;
    }
    /// <summary>
    /// ensure that this is called only when there is no duplicate for toAdd hashvalue in the tree and tree is not empity
    /// </summary>
    /// <param name="toAdd"></param>
    void redBlackInsertion(int toAdd)
    {

        setIsRedAtPos(toAdd, true);

        int parent = findHashParent(getHashAtPos(toAdd), indexlistHead, -1);
        setLastIndexAtPos(toAdd, parent);
        if (getHashAtPos(parent) > getHashAtPos(toAdd))
            setLeftChildIndexAtPos(parent, toAdd);
        else
            setRightChildIndexAtPos(parent, toAdd);
        if (getIsRedAtPos(parent))
            fixRBPropertyViolations(toAdd);
    }
    void fixRBPropertyViolations(int current)
    {
        if (getIsRedAtPos(current))
        {
            if (getLastIndexAtPos(current) == -1)
                setIsRedAtPos(current, false);//if this is root, just recolor it and double reds are solved
            else if (getIsRedAtPos(getLastIndexAtPos(current)))//if parent of current is black there is no double red violation
            {
                //if parent of current is red, then it has at least a grandparent
                int parent = getLastIndexAtPos(current);
                int uncle = Sibling(parent);
                int grandparent = getLastIndexAtPos(parent);

                if (uncle != -1)
                    if (getIsRedAtPos(uncle) && (!getIsRedAtPos(grandparent)))
                    {//case 1, just switch color and recur
                        setIsRedAtPos(parent, false);
                        setIsRedAtPos(uncle, false);
                        setIsRedAtPos(grandparent, true);
                        fixRBPropertyViolations(grandparent);
                        return;
                    }
                if (isInternal(current))
                {
                    //case 2
                    rotationSwap(parent, current);
                    fixRBPropertyViolations(parent);
                    return;
                }
                //case 3
                setIsRedAtPos(grandparent, true);
                setIsRedAtPos(parent, false);
                rotationSwap(grandparent, parent);
            }

        }

    }
    #endregion
    #region deletion
    public void Remove(int indexedArrayElement, int hashvalue)
    {
        if (indexlistHead == -1)
            throw new KeyNotFoundException();
        int holder = findHash(hashvalue, indexlistHead);
        if (holder == -1)
            throw new KeyNotFoundException();
        else
        {
            if (!indexerContains(holder, indexedArrayElement))
                throw new KeyNotFoundException();
            else
            {
                indexerRemove(holder, indexedArrayElement);
                if (getIndexedDataCountAtPos(holder) <= 0)
                {
                    Remove(holder);
                    --count;
                }

            }
        }
    }
    int getInOrderNextInSubTree(int node)
    {
        if (getRightChildIndexAtPos(node) == -1)
            return -1;

        int result = getRightChildIndexAtPos(node);
        while (getLeftChildIndexAtPos(result) != -1)
            result = getLeftChildIndexAtPos(result);
        return result;
    }
    int getInOrderPreviousInSubTree(int node)
    {
        if (getLeftChildIndexAtPos(node) == -1)
            return -1;

        int result = getLeftChildIndexAtPos(node);
        while (getRightChildIndexAtPos(result) != -1)
            result = getRightChildIndexAtPos(result);
        return result;
    }
    void Remove(int toRemove)
    {
        //if has 2 child, switch places with next.
        if (getRightChildIndexAtPos(toRemove) != -1 && getLeftChildIndexAtPos(toRemove) != -1)
        {
            int toSwitchWith = getInOrderNextInSubTree(toRemove);
            setHashIndexPos(toRemove, getHashAtPos(toSwitchWith));
            setIndexedArrayDataAtPos(toRemove, getIndexedArrayDataAtPos(toSwitchWith));
            Remove(toSwitchWith);
            return;
        }
        else
        {

            //if is red leaf, remove
            if (getRightChildIndexAtPos(toRemove) == -1 && getLeftChildIndexAtPos(toRemove) == -1)
            {
                int parent = getLastIndexAtPos(toRemove);
                if (parent != -1)//if isn't root
                {
                    int sibling = Sibling(toRemove);
                    if (getLeftChildIndexAtPos(parent) == toRemove)//if it's a left child
                    {
                        setLeftChildIndexAtPos(parent, -1);
                    }
                    else//if it's a right child
                    {
                        setRightChildIndexAtPos(parent, -1);
                    }

                    if (!getIsRedAtPos(toRemove))
                        solveDoubleBlack(parent, toRemove, sibling);
                }
                else
                {//if is root, and has no children, delete tree
                    indexlistHead = -1;
                }
                dispose(toRemove);

                return;
            }
            //if has one child
            if (getLeftChildIndexAtPos(toRemove) != -1)
                deleteWithChild(getLeftChildIndexAtPos(toRemove));
            else
                deleteWithChild(getRightChildIndexAtPos(toRemove));
        }

    }

    /// <summary>
    /// substitute the toDestroy node with the source node inside the tree, preserving all the content and label data of source, with flags for each tie
    /// </summary>
    /// <param name="source"></param>
    /// <param name="toDestroy"></param>
    void substitute(int source, int toDestroy, bool preserveLast = false, bool preserveLeft = false, bool preserveRight = false)
    {
        if (preserveLast)
        {
            if (getLastIndexAtPos(toDestroy) != -1)
            {
                if (getLeftChildIndexAtPos(getLastIndexAtPos(toDestroy)) == toDestroy)
                    setLeftChildIndexAtPos(getLastIndexAtPos(toDestroy), source);
                else
                    setRightChildIndexAtPos(getLastIndexAtPos(toDestroy), source);
            }
            setLastIndexAtPos(source, getLastIndexAtPos(toDestroy));
        }
        if (preserveRight)
        {
            if (getRightChildIndexAtPos(toDestroy) != -1)
            {
                setLastIndexAtPos(getRightChildIndexAtPos(toDestroy), source);
            }
            setRightChildIndexAtPos(source, getRightChildIndexAtPos(toDestroy));
        }
        if (preserveLeft)
        {
            if (getLeftChildIndexAtPos(toDestroy) != -1)
            {
                setLastIndexAtPos(getLeftChildIndexAtPos(toDestroy), source);
            }
            setLeftChildIndexAtPos(source, getLeftChildIndexAtPos(toDestroy));
        }
        dispose(toDestroy);

    }

    void deleteWithChild(int child)
    {
        if (getIsRedAtPos(child))
        {
            setIsRedAtPos(child, false);
            substitute(child, getLastIndexAtPos(child), preserveLast: true);
            return;
        }
        else
        {
            substitute(child, getLastIndexAtPos(child), preserveLast: true);
            int parent = getLastIndexAtPos(child);
            if (!getIsRedAtPos(parent))
            {
                //case when there's double black
                if (parent != -1)
                    solveDoubleBlack(parent, child, Sibling(child));
            }
        }
    }

    void solveDoubleBlack(int doubleBlackParent, int child, int sibling)
    {
        //if is root, stop it
        if (doubleBlackParent == -1)
            return;
        //if is chain with red parent, just make it black
        if (sibling == -1 && getIsRedAtPos(doubleBlackParent))
        { setIsRedAtPos(doubleBlackParent, false); return; }
        //if is chain with black parent, just recur on him
        if (sibling == -1 && !getIsRedAtPos(doubleBlackParent))
        { solveDoubleBlack(getLastIndexAtPos(doubleBlackParent), doubleBlackParent, Sibling(doubleBlackParent)); return; }

        //otherwise, both parent and sibling exist, deal with doubleblack
        recognizeDoubleBlackCase(doubleBlackParent, child, sibling);

    }

    void recognizeDoubleBlackCase(int Parent, int doubleBlack, int sibling)
    {
        //PrintPretty(Parent, " parent", true, false);
        //PrintPretty(doubleBlack, " doubleblack ", true, false);
        //PrintPretty(sibling, " sibling", true, false);
        if ((!getIsRedAtPos(Parent)) && (!blackOrNull(sibling)))
            solveCaseOne(Parent, doubleBlack, sibling);
        else if ((getIsRedAtPos(Parent)) && blackOrNull(sibling) && (blackOrNull(InternalChild(sibling)) && blackOrNull(ExternalChild(sibling))))
            solveCaseTwo(Parent, doubleBlack, sibling);
        else if ((!getIsRedAtPos(Parent)) && blackOrNull(sibling) && (blackOrNull(InternalChild(sibling)) && blackOrNull(ExternalChild(sibling))))
            solveCaseThree(Parent, doubleBlack, sibling);
        else if (blackOrNull(sibling) && ((!blackOrNull(InternalChild(sibling))) && blackOrNull(ExternalChild(sibling))))
            solveCaseFour(Parent, doubleBlack, sibling);
        else if (blackOrNull(sibling) && (!blackOrNull(ExternalChild(sibling))))
            solveCaseFive(Parent, doubleBlack, sibling);
        else
            throw new InvalidOperationException(" unrecognized case with p red:" + getIsRedAtPos(Parent) + " sibling blackornull:" + blackOrNull(sibling) + " internal blackornull:" + blackOrNull(InternalChild(sibling)) + " external blackornull:" + blackOrNull(ExternalChild(sibling)));
    }
    void solveCaseOne(int Parent, int doubleBlack, int sibling)
    {
        int futureSibling = InternalChild(sibling);
        bool temp = getIsRedAtPos(Parent);
        setIsRedAtPos(Parent, getIsRedAtPos(sibling));
        setIsRedAtPos(sibling, temp);
        rotationSwap(Parent, sibling);
        solveDoubleBlack(Parent, doubleBlack, futureSibling);
    }
    void solveCaseTwo(int Parent, int doubleBlack, int sibling)
    {
        setIsRedAtPos(sibling, true);
        setIsRedAtPos(Parent, false);
    }
    void solveCaseThree(int Parent, int doubleBlack, int sibling)
    {
        setIsRedAtPos(sibling, true);
        solveDoubleBlack(getLastIndexAtPos(Parent), Parent, Sibling(Parent));
    }
    void solveCaseFour(int Parent, int doubleBlack, int sibling)
    {
        int futureSibling = InternalChild(sibling);
        setIsRedAtPos(InternalChild(sibling), false);
        setIsRedAtPos(sibling, true);
        rotationSwap(sibling, InternalChild(sibling));
        printRBTree();
        isRBTree();
        solveCaseFive(Parent, doubleBlack, futureSibling);
    }
    void solveCaseFive(int Parent, int doubleBlack, int sibling)
    {
        setIsRedAtPos(sibling, getIsRedAtPos(Parent));
        setIsRedAtPos(Parent, false);
        setIsRedAtPos(ExternalChild(sibling), false);

        rotationSwap(Parent, sibling);

    }

    #endregion
    #region debug
    public bool isRBTree() { int pathcountvar = 0; return isRBTree(indexlistHead, out pathcountvar); }
    bool isRBTree(int subTreeHead, out int pathCount)
    {

        if (subTreeHead == -1)
        {
            pathCount = 0;
            return true;
        }
        int lpathCount;
        bool left = isRBTree(getLeftChildIndexAtPos(subTreeHead), out lpathCount);
        int rpathCount;
        bool right = isRBTree(getRightChildIndexAtPos(subTreeHead), out rpathCount);
        pathCount = lpathCount;

        if (getLastIndexAtPos(subTreeHead) == -1)
        {
            pathCount = 0;
            if (getIsRedAtPos(subTreeHead))
            {
                UnityEngine.Debug.LogError("Red root detected! on hash " + getHashAtPos(subTreeHead));
                return false;
            }
        }
        else
        {
            if (!getIsRedAtPos(subTreeHead))
            { ++pathCount; }
            else
                if (getIsRedAtPos(getLastIndexAtPos(subTreeHead)))
            {
                UnityEngine.Debug.LogError("Double red detected! on hash " + getHashAtPos(subTreeHead));
                return false;
            }
        }
        if (lpathCount != rpathCount)
            UnityEngine.Debug.LogError("black child count not matching on hash " + getHashAtPos(subTreeHead));

        return (lpathCount == rpathCount && left && right);

    }
    public void printRBTree()
    {
        if (indexlistHead == -1)
            UnityEngine.Debug.Log(" empity ---");
        else
        {
            UnityEngine.Debug.Log(" BST: ");
            PrintPretty(indexlistHead, "", true, true);
        }

    }
    public void PrintPretty(int index, string indent, bool last, bool recursive)
    {
        string result = indent;
        if (last)
        {
            result += "\\-";
            indent += "  ";
        }
        else
        {
            result += "|-";
            indent += "| ";
        }
        if (getLastIndexAtPos(index) != -1)
        {
            result += (getLeftChildIndexAtPos(getLastIndexAtPos(index)) == index) ? "L->" : "R->";
        }
        int arraycount = getIndexedArrayDataAtPos(index).Length;
        result += getHashAtPos(index) + (getIsRedAtPos(index) ? " _R_" : " _B_") + " content size " + arraycount;
        for (int i = 0; i < arraycount; i++)
        {
            if (getIndexedArrayDataAtPos(index, i) != -1)
                result += " [" + i + "]=" + getIndexedArrayDataAtPos(index, i) + "|";
        }
        UnityEngine.Debug.Log(result);
        if (recursive)
        {
            if (getLeftChildIndexAtPos(index) != -1)
                PrintPretty(getLeftChildIndexAtPos(index), indent, getRightChildIndexAtPos(index) == -1, true);
            if (getRightChildIndexAtPos(index) != -1)
                PrintPretty(getRightChildIndexAtPos(index), indent, true, true);
        }

    }
    #endregion

    #region arrayWrap
    int getIndexedDataCountAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].ContentCount;
    }
    void resizeIndexedDataCountAtPos(int key)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].resize();
    }
    void resetIndexedDataCountAtPos(int key)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].ContentCount = 0;
    }
    void incrementIndexedDataCountAtPos(int key)
    {
        ++ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].ContentCount;
    }
    void decrementIndexedDataCountAtPos(int key)
    {
        --ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].ContentCount;
    }
    int[] getIndexedArrayDataAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].indexedArrayData;
    }
    void setIndexedArrayDataAtPos(int key, int[] indexedArrayData)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].indexedArrayData = indexedArrayData;
    }
    int getIndexedArrayDataAtPos(int key, int subPos)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].indexedArrayData[subPos];
    }
    void setIndexedArrayDataIndexPos(int key, int subPos, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].indexedArrayData[subPos] = value;
    }
    int getHashAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].hashValue;
    }
    void setHashIndexPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].hashValue = value;
    }
    int getRightChildIndexAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].rightChildIndex;
    }
    void setRightChildIndexAtPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].rightChildIndex = value;
    }
    int getLeftChildIndexAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].leftChildIndex;
    }
    void setLeftChildIndexAtPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].leftChildIndex = value;
    }

    int getLastIndexAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].lastIndex;
    }
    void setLastIndexAtPos(int key, int value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].lastIndex = value;
    }

    bool getIsRedAtPos(int key)
    {
        return ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].isRed;
    }
    void setIsRedAtPos(int key, bool value)
    {
        ArrayData[key / DEFAULTCAPACITY][key % DEFAULTCAPACITY].isRed = value;
    }
    #endregion
}
