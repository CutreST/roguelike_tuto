using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Extension methods for godot
/// </summary>
public static class ExtensionMethods
{
    #region Vector2

    /// <summary>
    /// Vector2 zero
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>    
    public static Vector2 Zero(this Vector2 v)
    {
        return new Vector2(0f, 0f);
    }

    #endregion
    /// <summary>
    /// Recursive search to get a parent by it's type
    /// </summary>
    /// <typeparam name="T">Type of the desired parent</typeparam>
    /// <param name="n"></param>
    /// <returns>The parent, null if not found</returns>
    public static T TryGetFromParent_Rec<T>(this Node n) where T : class
    {
        if (n == null)
        {
            return null;
        }

        if (n.GetParent() is T == false)
        {
            return TryGetFromParent_Rec<T>(n.GetParent());
        }
        else if (n == n.GetTree().Root)
        {
            return null;
        }

        return n.GetParent() as T;
    }

    /// <summary>
    /// Recursive search to get a child by it's type
    /// </summary>
    /// <typeparam name="T">Type of the desired child</typeparam>
    /// <param name="n"></param>
    /// <returns>The child, null if not found</returns>
    public static T TryGetFromChild_Rec<T>(this Node n) where T : Node
    {
        if (n == null)
        {
            return null;
        }

        T temp;

        for (int i = 0; i < n.GetChildCount(); i++)
        {
            if (n.GetChild(i) is T)
            {
                return n.GetChild(i) as T;
            }
            else
            {
                temp = n.GetChild(i).TryGetFromChild_Rec<T>();

                if(temp != null){
                    return temp;
                }                
            }

        }
        return null;

    }

     /// <summary>
    /// Recursive search to get a child by it's type and name
    /// </summary>
    /// <typeparam name="T">Type of the desired child</typeparam>
    /// <param name="n"></param>
    /// <returns>The child, null if not found</returns>
    public static T TryGetFromChild_Rec<T>(this Node n, in string name) where T : Node
    {
        if (n == null)
        {
            return null;
        }

        T temp;         
        for (int i = 0; i < n.GetChildCount(); i++)
        {
            if (n.GetChild(i).Name == name && n.GetChild(i) is T)
            {           
                return n.GetChild(i) as T;
            }
            else
            {
                temp = n.GetChild(i).TryGetFromChild_Rec<T>(name);

                if(temp != null){
                    return temp;
                }                
            }

        }
        return null;

    }

    /// <summary>
    /// Gets a full list of children by the desired tye
    /// </summary>
    /// <typeparam name="T">Type of the children</typeparam>
    /// <param name="n"></param>
    /// <returns>A list of children by type</returns>
    public static List<T> TryGetFromChildren_Rec<T>(this Node n) where T : Node{
        List<T> children = new List<T>();

        if(n == null){
            return children;
        }

        T temp;        
        for(int i = 0; i < n.GetChildCount(); i++){
            if( (temp = n.GetChild(i) as T) != null){
                children.Add(temp);
            }

            children.AddRange(n.GetChild(i).TryGetFromChildren_Rec<T>());            
        }

        return children;
    }     

    /// <summary>
    /// Centers the game window on the screen.
    /// </summary>
    public static void CenterWindow(){
        OS.WindowPosition = (OS.GetScreenSize()/2) - (OS.WindowSize/2);
    }

    public static int GetPureSign(in float number){
        if(number < 0){
            return -1;
        }else if(number > 0){
            return 1;
        }
        else{
            return 0;
        }
    }
}
