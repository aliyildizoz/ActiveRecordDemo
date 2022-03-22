using System;
using System.Collections.Generic;
using System.Linq;
using ActiveRecordDemo.ActiveRecords;

namespace ActiveRecordDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("************* Create ***********");
            Person createPerson1 = new Person() { Age = 23, FirstName = "Ali", LastName = "Yıldızöz" };
            Person createPerson2 = new Person() { Age = 23, FirstName = "Ahmet", LastName = "Akdoğan" };
            createPerson1.Save();
            createPerson2.Save();
            Console.WriteLine($"createPerson1 Id: {createPerson1.Id}");
            Console.WriteLine($"createPerson2 Id: {createPerson2.Id}");

            Console.WriteLine("\n\n************* Read ***********");
            List<Person> personList = BaseActiveRecord<Person>.Read().ToList();
            foreach (var person in personList)
            {
                Console.WriteLine($"{person.Id}-{person.FirstName}-{person.LastName}-{person.Age}");
            }

            Console.WriteLine("\n\n************* Update ***********");
            Person updatePerson = new Person() { Id = 5, Age = 22, FirstName = "Test-Update", LastName = "Yıldız" };
            updatePerson.Update();
            Person updated = BaseActiveRecord<Person>.FindById(updatePerson.Id);
            Console.WriteLine($"{updated.Id}-{updated.FirstName}-{updated.LastName}-{updated.Age}");

            Console.WriteLine("\n\n************* Delete ***********");
            Person deletePerson = new Person() { Id = 5 };
            deletePerson.Delete();
            Person deleted = BaseActiveRecord<Person>.FindById(deletePerson.Id);
            Console.WriteLine($"IsDeleted for id={deletePerson.Id}:{deleted == null}");

            Console.WriteLine("\n\n************* Read2 ***********");
            List<Person> personList2 = BaseActiveRecord<Person>.Read().ToList();
            foreach (var person in personList2)
            {
                Console.WriteLine($"{person.Id}-{person.FirstName}-{person.LastName}-{person.Age}");
            }

        }

    }
}
