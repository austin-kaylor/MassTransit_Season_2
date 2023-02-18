using System;
using MassTransit;

namespace Library.Components.StateMachines;

/*
* This is the data that is actually stored in the Saga Repository
* Data is used by the State Machine
*/
public class Book : SagaStateMachineInstance
{
    /*
     * Required. Stores the current state of the given SagaInstance
     * Most common way people store is either an integer or a string
     * string gets big and fluffy, depending on what states are defined. example: BookWasCheckedOutButNotReturned
     * ints -> need to take extra care that you've assigned unique values for each state. example: can't be more than 1 state mapped to 3
     */
    public int CurrentState { get; set; }

    /// <summary>
    /// Timestamp of when the book was added to the Library
    /// </summary>
    public DateTime DateAdded { get; set; }

    /// <summary>
    /// Title of the book
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// ISBN https://en.wikipedia.org/wiki/ISBN
    /// </summary>
    public string Isbn { get; set; }

    /// <summary>
    /// Required. Default Primary Key for a Saga Instance. All Saga Instances are correlated by a correlation GUID
    /// </summary>
    public Guid CorrelationId { get; set; }
}