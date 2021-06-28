using Godot;
using System;
using Entities.Components;
using MySystems;

/// <summary>
/// Class that spam different messages.
/// Used to unify all the messages needed for debug.
/// </summary>
public static class Messages
{
    /// <summary>
    /// The type of message desired
    /// </summary>
    public enum MessageType : byte { LOG, ERROR}

    private static string NODE_SAY_FORMAT = "[{0} says]:";
    private static string PREF_ERROR = "***ERROR: ";
    private static string PREF_OJU = "***OJU: ";
    /// <summary>
    /// Prints a message based on the type
    /// </summary>
    /// <param name="message">Message to be printed</param>
    /// <param name="mesType">Type of message</param>
    public static void Print(in string message, in MessageType mesType = MessageType.LOG)
    {
        switch (mesType)
        {
            case MessageType.ERROR:
                GD.PrintErr(String.Concat(PREF_ERROR, message));
                break;

            case MessageType.LOG:
                GD.PrintErr(String.Concat(PREF_OJU, message));
                break;
        }
    }

    public static void Print(in string name, in string message, in MessageType mesType = MessageType.LOG){
        
        switch (mesType)
        {
            case MessageType.ERROR:
                GD.PrintErr(String.Format(String.Concat(PREF_ERROR, NODE_SAY_FORMAT, message), name));
                break;

            case MessageType.LOG:
                GD.PrintErr(String.Format(String.Concat(PREF_OJU, NODE_SAY_FORMAT, message), name));
                break;
        }
    }

    #region MySystem Messages
    /// <summary>
    /// Mesage used at <see cref="System_Base.OnEnterSystem(object[])"/>
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void EnterSystem<T>(in T system, in MessageType mesType = MessageType.LOG) where T : System_Base
    {        
        string temp = string.Concat(typeof(T).Name, " has entered the list");
        Print(temp, mesType);
       
    }

    /// <summary>
    /// Mesage used at <see cref="System_Base.OnExitSystem(object[])"/>
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void ExitSystem<T>(in T system, in MessageType mesType = MessageType.LOG) where T : System_Base
    {
        string temp = string.Concat(typeof(T).Name, " has exited the list");
        Print(temp, mesType);
    }

    /// <summary>
    ///<para>OJUUUU! Hay que cambiar el sistema porque si no lo pilla, no funca</para>
    /// Mesage used at <see cref="System_Manager.TryGetSystem{T}(out T, bool, object[])"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void GetSystemFailed<T>(in T system, in Entities.Entity parent, in MessageType mesType = MessageType.LOG) where T : System_Base
    {
        string temp = string.Concat(typeof(T).Name, " called by ", parent.Name, " not found on system list");
        Print(temp, mesType);
    }

    /// <summary>
    /// Mesage used at <see cref="System_Manager.TryAddSystem{T}(out T, bool, object[])"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void AddSystemFailed<T>(in T system, in string origin, in MessageType mesType = MessageType.LOG) where T : System_Base
    {
        string temp = string.Concat(typeof(T).Name, " called by ", origin, " not found on system list");
        Print(temp, mesType);
    }


    #endregion

    #region Stack Messages
    /// <summary>
    /// Mesage used at <see cref="Visual_SystemBase.OnEnterStack"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void EnterStack(in Visual_SystemBase b, in MessageType mesType = MessageType.LOG)
    {
        string temp = string.Concat(b.GetType().Name, " has entered the stack");
        Print(temp, mesType);
    }

    /// <summary>
    /// Mesage used at <see cref="Visual_SystemBase.OnExitStack"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void ExitStack(in Visual_SystemBase b, in MessageType mesType = MessageType.LOG)
    {
        string temp = string.Concat(b.GetType().Name, " has exited the stack");
        Print(temp, mesType);
    }


    /// <summary>
    /// Mesage used at <see cref="Visual_SystemBase.OnResumeStack"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void ResumeStack(in Visual_SystemBase b, in MessageType mesType = MessageType.LOG)
    {
        string temp = string.Concat(b.GetType().Name, " has resumed at the stack");
        Print(temp, mesType);

    }


    /// <summary>
    /// Mesage used at <see cref="Visual_SystemBase.OnPauseStack"/> fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void PauseStack(in Visual_SystemBase b, in MessageType mesType = MessageType.LOG)
    {
        string temp = string.Concat(b.GetType().Name, " has paused on the stack");
        Print(temp, mesType);
    }
    #endregion

   

     /// <summary>
    /// Mesage used when a component fails
    /// </summary>
    /// <param name="s">the system that spans the message</param>
    /// <param name="mesType">TYhe type of the message. <see cref="MessageType.LOG"/> by defualt</param>
    public static void GetComponentFailed(in string compName, in string origin, in MessageType mesType = MessageType.LOG)
    {
        string temp = string.Concat(compName, " called by ", origin, " not found on entity");
        Print(temp, mesType);
    }



}
