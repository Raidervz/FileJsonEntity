using System;
using System.Collections.Generic;
using System.IO;
using FileEntity.Core;

namespace FileEntity.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string Encriptphase = "*&GO^6f46";
            bool UseEncription = true;
            string FileName = "ModelTest.data";
            string MyDir = System.AppContext.BaseDirectory;

            File.Delete(MyDir+FileName);

            var ModelTestEntityTable = new FileEntity<ModelTest>(FileName, MyDir,UseEncription,Encriptphase);

            ModelTest FirstModelTest = new ModelTest() {
                ID = 1,
                age = 18,
                lastname = "beauty",
                name = "indecice"
            };

            ModelTest SecondModelTest = new ModelTest() {
                ID = 2,
                age = 24,
                lastname = "beauty",
                name = "perfect"
            };

            ModelTest ThirdModelTest = new ModelTest() {
                ID = 3,
                age = 40,
                lastname = "sexy",
                name = "wise"
            };

            ModelTest FourModelTest = new ModelTest() {
                ID = 4,
                age = 60,
                lastname = "woman",
                name = "Crazy"
            };

            List<ModelTest> modelTestList;

            //Adding records to the Table
            ModelTestEntityTable.Add(FirstModelTest);
            ModelTestEntityTable.Add(SecondModelTest);
            ModelTestEntityTable.Add(ThirdModelTest);            
            ModelTestEntityTable.Add(FourModelTest);


            //Find All the Items
            modelTestList = ModelTestEntityTable.FindAll();                      

            Console.WriteLine("ModelTestListCount :" + modelTestList.Count.ToString());

            Console.WriteLine("ModelTest Records.");

            modelTestList.ForEach(delegate(ModelTest model) {
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Console.Write(nameof(ModelTest.ID) + ": ");
                Console.WriteLine(model.ID);
                Console.Write(nameof(ModelTest.name) + ": ");
                Console.WriteLine(model.name);
                Console.Write(nameof(ModelTest.lastname) + ": ");
                Console.WriteLine(model.lastname);
                Console.Write(nameof(ModelTest.age) + ": ");
                Console.WriteLine(model.age);
            });

            //Editing SecondModelTest Record
            SecondModelTest.name = "The " + SecondModelTest.name;
            ModelTestEntityTable.Update(SecondModelTest, nameof(ModelTest.ID));

            modelTestList = ModelTestEntityTable.FindAll();                      

            Console.WriteLine("ModelTestListCount :" + modelTestList.Count.ToString());

            Console.WriteLine("ModelTest Records.");

            modelTestList.ForEach(delegate(ModelTest model) {
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Console.Write(nameof(ModelTest.ID) + ": ");
                Console.WriteLine(model.ID);
                Console.Write(nameof(ModelTest.name) + ": ");
                Console.WriteLine(model.name);
                Console.Write(nameof(ModelTest.lastname) + ": ");
                Console.WriteLine(model.lastname);
                Console.Write(nameof(ModelTest.age) + ": ");
                Console.WriteLine(model.age);
            });


            //Deleting ThirdModelTest Record,
            ModelTestEntityTable.Delete(ThirdModelTest, nameof(ModelTest.ID));
            modelTestList = ModelTestEntityTable.FindAll();                      

            Console.WriteLine("ModelTestListCount :" + modelTestList.Count.ToString());

            Console.WriteLine("ModelTest Records.");

            modelTestList.ForEach(delegate(ModelTest model) {
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Console.Write(nameof(ModelTest.ID) + ": ");
                Console.WriteLine(model.ID);
                Console.Write(nameof(ModelTest.name) + ": ");
                Console.WriteLine(model.name);
                Console.Write(nameof(ModelTest.lastname) + ": ");
                Console.WriteLine(model.lastname);
                Console.Write(nameof(ModelTest.age) + ": ");
                Console.WriteLine(model.age);
            });

            //FindFirst that contain
            var modelTestFindFirst = ModelTestEntityTable.FindFirst(new Dictionary<string, string>() { {"name", "Crazy"} } );
            Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            Console.Write(nameof(ModelTest.ID) + ": ");
            Console.WriteLine(modelTestFindFirst.ID);
            Console.Write(nameof(ModelTest.name) + ": ");
            Console.WriteLine(modelTestFindFirst.name);
            Console.Write(nameof(ModelTest.lastname) + ": ");
            Console.WriteLine(modelTestFindFirst.lastname);
            Console.Write(nameof(ModelTest.age) + ": ");
            Console.WriteLine(modelTestFindFirst.age);

            //FindMany
            var modelTestFindMany = ModelTestEntityTable.FindMany(new Dictionary<string, string>() { {"lastname", "beauty"} } );
            Console.WriteLine("modelTestFindMany :" + modelTestFindMany.Count.ToString());

            Console.WriteLine("ModelTest Records.");

            modelTestFindMany.ForEach(delegate(ModelTest model) {
                Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Console.Write(nameof(ModelTest.ID) + ": ");
                Console.WriteLine(model.ID);
                Console.Write(nameof(ModelTest.name) + ": ");
                Console.WriteLine(model.name);
                Console.Write(nameof(ModelTest.lastname) + ": ");
                Console.WriteLine(model.lastname);
                Console.Write(nameof(ModelTest.age) + ": ");
                Console.WriteLine(model.age);
            });
        }
    }
}
