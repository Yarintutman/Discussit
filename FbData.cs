﻿using Android.App;
using Android.Gms.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Java.Util;
using System;
using System.Collections.Generic;

namespace Discussit
{
    /// <summary>
    /// This class encapsulates Firebase authentication and Firestore database initialization for data management in a Firebase-based application.
    /// </summary>
    internal class FbData
    {
        private readonly FirebaseAuth auth;
        private readonly FirebaseFirestore firestore;

        /// <summary>
        /// Initializes a new instance of the FbData class.
        /// If the Firebase app is not already initialized, it initializes it with the provided options.
        /// </summary>
        public FbData()
        {
            Firebase.FirebaseApp app = Firebase.FirebaseApp.InitializeApp(Application.Context);
            if (app is null)
            {
                FirebaseOptions options = GetMyOptions();
                app = Firebase.FirebaseApp.InitializeApp(Application.Context, options);
            }
            auth = FirebaseAuth.Instance;
            firestore = FirebaseFirestore.GetInstance(app);
        }

        /// <summary>
        /// Constructs and returns FirebaseOptions containing configuration details for Firebase services.
        /// These details include Project ID, Application ID, API Key, and Storage Bucket.
        /// </summary>
        /// <returns>FirebaseOptions object containing Firebase configuration options.</returns>
        private FirebaseOptions GetMyOptions()
        {
            return new FirebaseOptions.Builder()
                    .SetProjectId("discussit-37dba")
                    .SetApplicationId("1:560980864181:android:a8da7d40a4d9b4891ba2fc")
                    .SetApiKey("AIzaSyB3v_TsmY8zFLBme5hoABPW-NC7LXTdvJo")
                    .SetStorageBucket("discussit-37dba.appspot.com")
                    .Build();
        }

        /// <summary>
        /// Registers a new user with the provided email and password using Firebase authentication.
        /// </summary>
        /// <param name="email">The email address of the user to be registered.</param>
        /// <param name="password">The password of the user to be registered.</param>
        /// <returns>A Task representing the asynchronous operation of user registration.</returns>
        public Task Register(string email, string password)
        {
            return auth.CreateUserWithEmailAndPassword(email, password);
        }

        /// <summary>
        /// Logs in an existing user with the provided email and password using Firebase authentication.
        /// </summary>
        /// <param name="email">The email address of the user to be logged in.</param>
        /// <param name="password">The password of the user to be logged in.</param>
        /// <returns>A Task representing the asynchronous operation of user login.</returns>
        public Task Login(string email, string password)
        {
            return auth.SignInWithEmailAndPassword(email, password);
        }

        /// <summary>
        /// Sends a password reset email to the specified email address.
        /// </summary>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns>A task representing the asynchronous operation, which upon completion, indicates whether the password reset email was sent successfully.</returns>
        public Task ResetPassword(string email)
        {
            return auth.CurrentUser.Email == email? auth.SendPasswordResetEmail(email) : null;
        }

        /// <summary>
        /// Retrieves the unique identifier (UID) of the currently authenticated user.
        /// </summary>
        /// <returns>A string representing the UID of the currently authenticated user, or an empty string if no user is authenticated.</returns>
        public string GetUserId()
        {
            // Check if there is a currently authenticated user
            // If authenticated, return the UID of the user; otherwise, return an empty string
            return auth.CurrentUser is null ? string.Empty : auth.CurrentUser.Uid;
        }

        /// <summary>
        /// Sets the contents of a Firestore document with the provided data.
        /// If the document ID is not empty, updates the existing document; otherwise, creates a new document.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to be set or updated. If empty, a new document will be created.</param>
        /// <param name="newId">Output parameter to hold the ID of the newly created or updated document.</param>
        /// <param name="hm">A HashMap containing the data to be set in the document.</param>
        /// <returns>A Task representing the asynchronous operation of setting the document.</returns>
        public Task SetDocument(string cName, string docId, out string newId, HashMap hm)
        {
            Task t;
            if (docId != string.Empty)
            {
                t = firestore.Collection(cName).Document(docId).Set(hm);
                newId = docId;
            }
            else
            {
                DocumentReference dr = firestore.Collection(cName).Document();
                newId = dr.Id;
                t = dr.Set(hm);
            }
            return t;
        }

        /// <summary>
        /// Deletes a Firestore document with the specified ID from the given collection.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to be deleted.</param>
        /// <returns>A Task representing the asynchronous operation of deleting the document.</returns>
        public Task DeleteDocument(string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).Delete();
        }

        /// <summary>
        /// Retrieves the contents of a Firestore document with the specified ID from the given collection.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to be retrieved.</param>
        /// <returns>A Task representing the asynchronous operation of getting the document.</returns>
        /// 
        public Task GetDocument(string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).Get();
        }

        /// <summary>
        /// Retrieves documents from a Firestore collection where the document ID is in the specified list.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection.</param>
        /// <param name="lst">A list of document IDs to filter by.</param>
        /// <returns>A Task representing the asynchronous operation of fetching documents.</returns>
        public Task GetDocumentsInList(string cName, IList<Java.Lang.Object> lst)
        {
            return firestore.Collection(cName).WhereIn(Firebase.Firestore.FieldPath.DocumentId(), lst).Get();
        }

        /// <summary>
        /// Retrieves all documents from a Firestore collection.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection.</param>
        /// <returns>A Task representing the asynchronous operation of fetching documents.</returns>
        public Task GetCollection(string cName)
        {
            return firestore.Collection(cName).Get();
        }

        /// <summary>
        /// Retrieves documents from a collection that match the specified field value and returns them in order of the specified field, limited by the specified count.
        /// </summary>
        /// <param name="cName">The name of the collection.</param>
        /// <param name="fNameWhere">The name of the field to filter by.</param>
        /// <param name="fValue">The value to filter documents by.</param>
        /// <param name="fNameOrderBy">The name of the field to order documents by.</param>
        /// <param name="limit">The maximum number of documents to retrieve.</param>
        /// <returns>A task representing the asynchronous operation, which upon completion, contains the documents that meet the specified criteria.</returns>
        public Task GetHighestValue(string cName, string fNameWhere, Java.Lang.Object fValue, string fNameOrderBy, int limit)
        {
            return firestore.Collection(cName).WhereEqualTo(fNameWhere, fValue).OrderBy(fNameOrderBy).Limit(limit).Get();
        }

        /// <summary>
        /// Increments a numeric field in a Firestore document by a specified value.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to update.</param>
        /// <param name="fName">The name of the field to increment.</param>
        /// <param name="increment">The value by which to increment the field.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task IncrementField(string cName, string docId, string fName, int increment)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.Increment(increment));
        }

        /// <summary>
        /// Retrieves documents from a Firestore collection group where a specified field is equal to a given value.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection group containing the document.</param>
        /// <param name="fName">The name of the field to filter by.</param>
        /// <param name="fValue">The value to filter for.</param>
        /// <returns>A Task representing the asynchronous operation of fetching documents.</returns>
        public Task GetEqualToDocsInGroup(string cName, string fName, Java.Lang.Object fValue)
        {
            return firestore.CollectionGroup(cName).WhereEqualTo(fName, fValue).Get();
        }

        /// <summary>
        /// Updates a field in a Firestore document with the specified value.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to update.</param>
        /// <param name="fName">The name of the field to update.</param>
        /// <param name="fValue">The new value for the field.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task UpdateField(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, fValue);
        }

        /// <summary>
        /// Updates multiple fields in a Firestore document with the specified values.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to update.</param>
        /// <param name="fields">A dictionary containing field names as keys and corresponding values.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task UpdateDocument(string cName, string docId, IDictionary<string, Java.Lang.Object> fields)
        {
            return firestore.Collection(cName).Document(docId).Update(fields);
        }

        /// <summary>
        /// Adds an element to an array field in a Firestore document, creating the array if it does not exist.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document containing the array field.</param>
        /// <param name="fName">The name of the array field to update.</param>
        /// <param name="fValue">The value to add to the array.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task UnionArray(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.ArrayUnion(fValue));
        }

        /// <summary>
        /// Removes an element from an array field in a Firestore document.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document containing the array field.</param>
        /// <param name="fName">The name of the array field to update.</param>
        /// <param name="fValue">The value to remove from the array.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task RemoveFromArray(string cName, string docId, string fName, Java.Lang.Object fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.ArrayRemove(fValue));
        }

        /// <summary>
        /// Removes a List of elements from an array field in a Firestore document.
        /// </summary>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document containing the array field.</param>
        /// <param name="fName">The name of the array field to update.</param>
        /// <param name="fValue">The List of elements to remove from the array.</param>
        /// <returns>A Task representing the asynchronous operation of updating the document.</returns>
        public Task RemoveElementsFromArray(string cName, string docId, string fName, List<Java.Lang.Object> fValue)
        {
            return firestore.Collection(cName).Document(docId).Update(fName, FieldValue.ArrayRemove(fValue.ToArray()));
        }

        /// <summary>
        /// Adds a listener to a Firestore collection to receive real-time updates.
        /// </summary>
        /// <param name="activity">The activity where the listener is added.</param>
        /// <param name="cName">The name of the Firestore collection.</param>
        /// <returns>An IListenerRegistration representing the registration of the listener.</returns>
        public IListenerRegistration AddSnapshotListener(Activity activity, string cName)
        {
            return firestore.Collection(cName).AddSnapshotListener((Firebase.Firestore.IEventListener)activity);
        }

        /// <summary>
        /// Adds a listener to a specific Firestore document to receive real-time updates.
        /// </summary>
        /// <param name="activity">The activity where the listener is added.</param>
        /// <param name="cName">The name of the Firestore collection containing the document.</param>
        /// <param name="docId">The ID of the document to listen to.</param>
        /// <returns>An IListenerRegistration representing the registration of the listener.</returns>
        public IListenerRegistration AddSnapshotListener(Activity activity, string cName, string docId)
        {
            return firestore.Collection(cName).Document(docId).AddSnapshotListener((Firebase.Firestore.IEventListener)activity);
        }

        /// <summary>
        /// Converts a DateTime object to a Firestore Timestamp object.
        /// </summary>
        /// <param name="dt">The DateTime object to convert.</param>
        /// <returns>A Java.Sql.Timestamp representing the Firestore Timestamp.</returns>
        public Java.Sql.Timestamp DateTimeToFirestoreTimestamp(DateTime dt)
        {
            dt = TimeZoneInfo.ConvertTimeToUtc(dt);

            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan ts = dt - epoch;

            return new Java.Sql.Timestamp((long)ts.TotalMilliseconds);
        }

        /// <summary>
        /// Converts a Firestore Timestamp object to a DateTime object.
        /// </summary>
        /// <param name="ts">The Firestore Timestamp to convert.</param>
        /// <returns>A DateTime object representing the converted timestamp.</returns>
        public DateTime FirestoreTimestampToDateTime(Timestamp ts)
        {
            Java.Util.Date d = ts.ToDate();

            DateTime epoch = new DateTime(1970, 1, 1);
            DateTime dt = epoch.AddMilliseconds(d.Time);

            return TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
        }
    }
}