using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //To create a database
            MongoCRUD db = new MongoCRUD("AddressBook");

            //PersonModel person = new PersonModel
            //{
            //  FirstName = "Joe",
            //LastName = "smith",
            //     PrimaryAddress = new AddressModel
            //     {
            //         StreetAddress = "101 Oak street",
            //         City = "Scranton",
            //         State = "PA",
            //         ZipCode = "18322"
            //    }
            //};


            //db.InsertRecord("Users", person);
            //var recs = db.LoadRecord<PersonModel>("Users");

            //foreach(var rec in recs)
            //{
            //    Console.WriteLine($"{rec.Id}: {rec.FirstName} {rec.LastName}");

            //    if(rec.PrimaryAddress != null)
            //    {
            //        Console.WriteLine(rec.PrimaryAddress.City);
            //    }
            //}

            //var single = db.LoadRecordById<PersonModel>("Users",)



            Console.ReadLine();
        }
    }

    public class PersonModel
    {
        [BsonId] //this will prevent it from storing as _id
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AddressModel PrimaryAddress { get; set; }
    }

    public class AddressModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient("mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass&ssl=false");
            db = client.GetDatabase(database);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecord<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);
            var result = collection.ReplaceOne(new BsonDocument("_id", id),
                record,
                new UpdateOptions { IsUpsert = true });

        }

        public void DeleteRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    
    }
}
