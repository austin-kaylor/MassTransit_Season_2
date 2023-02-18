using System.Collections.Generic;
using Library.Components.StateMachines;
using Library.Contracts;
using MassTransit;

namespace Library.Components.StateMachines;

// ReSharper disable UnassignedGetOnlyAutoProperty MemberCanBePrivate.Global
public class BookStateMachine : MassTransitStateMachine<Book>
{
    public BookStateMachine()
    {
        /* 
         * Required for StateMachine to work
         * 
         * On the BookAdded Event (down below), when MassTransit receives a new BookAdded message,
         * it will go out to the SagaRepository and say, in loose SQL terms, get a book from the SagaRepository that matches
         * the book ID passed in the BookAdded message
         *
         * If the book Id in the BookAdded Message does not yet exist in the SagaRepository, MassTransit will create a new
         * Book SagaInstance, inserting the book with it's correlation Id = book Id in the BookAdded message
         */
        Event(() => Added, x => x.CorrelateById(m => m.Message.BookId));

        /*
         * Required for StateMachine to work
         * 
         * Defines states for the Book SagaInstance.
         * Maps the Available state to 3, because 1 = Initial, and 2 = Completed, which are automatically reserved
         * 
         * Always keep the states in order (use an enum) for a given Saga Repository. 
         * If you change them, all the records in your saga repository won't match the new state values without manual intervention.
         * i.e. updating a ton of rows.
         *
         * If you want to remove a state, the recommended approach is to obsolete the state but keep it where it is in the InstanceState list
         *
         * If you use strings for state, you don't have to worry about any of this.
         * MassTransit simply writes whatever the string name is to the SagaRepository. <- Seems a bit more durable to changes
         */
        InstanceState(x => x.CurrentState, Available);

        /*
         * STATE MACHINE BEHAVIOR
         */
            
        /*
         * Basically amounts to:
         * - When a BookAdded Event is received
         * - The BookAdded does not correlate (book ID) to an existing saga instance
         * - Create a new Book SagaInstance (think of it as a new row in the SagaRepository??)
         * - Then do the following:
         *      1. Copy the contents of the BookAdded message to the SagaRepository
         *      2. Transition the Book's SagaInstance state to Available
         */
        Initially(
            When(Added)
                .CopyDataToInstance()
                .TransitionTo(Available));

        DuringAny(
            When(Added)
                .CopyDataToInstance());
    }

    public Event<BookAdded> Added { get; }

    public State Available { get; }
}


public static class BookStateMachineExtensions
{
    public static EventActivityBinder<Book, BookAdded> CopyDataToInstance(
        this EventActivityBinder<Book, BookAdded> binder)
    {
        return binder.Then(x =>
        {
            x.Saga.DateAdded = x.Message.Timestamp.Date;
            x.Saga.Title = x.Message.Title;
            x.Saga.Isbn = x.Message.Isbn;
        });
    }
}