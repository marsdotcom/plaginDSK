using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;


namespace plaginDSK
{

    public class Commands : IExtensionApplication
    {

        Form1 F;
        // эта функция будет вызываться при выполнении в AutoCAD команды "TestCommand"
        [CommandMethod("form1")]
        public void MyCommand()
        {
            F = new Form1();
            if (F.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AddLightweightPolyline();
            F.Dispose();
        }
             

        [CommandMethod("netpoliline")]
        public void AddLightweightPolyline()
        {
            // Получение текущего документа и базы данных
            Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Старт транзакции
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Открытие таблицы Блоков для чтения
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                                            OpenMode.ForRead) as BlockTable;

                // Открытие записи таблицы Блоков пространства Модели для записи
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                                                 OpenMode.ForWrite) as BlockTableRecord;

                LayerTable acLyrTbl;
                acLyrTbl = acTrans.GetObject(acCurDb.LayerTableId,
                                             OpenMode.ForRead) as LayerTable;


                string progectLayerName = "progectLayer";
                string factLayerName = "factLayer";
                string mainLayerName = "mainLayer";

                if (acLyrTbl.Has(progectLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 1),
                        Name = progectLayerName
                    };
                    acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }                

                if (acLyrTbl.Has(factLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord
                    {
                        Color = Color.FromColorIndex(ColorMethod.ByAci, 4),
                        Name = factLayerName
                    };
                    acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                if (acLyrTbl.Has(mainLayerName) == false)
                {
                    LayerTableRecord acLyrTblRec = new LayerTableRecord
                    {                                           
                        Name = mainLayerName
                    };
                    acLyrTbl.UpgradeOpen();
                    acLyrTbl.Add(acLyrTblRec);
                    acTrans.AddNewlyCreatedDBObject(acLyrTblRec, true);
                }

                Polyline projectPline = new Polyline();
                projectPline.SetDatabaseDefaults();
                Polyline factPline = new Polyline();
                projectPline.SetDatabaseDefaults();

                int i = 0;
                double ygraf = 160,
                        ydelta = 130,
                        yproject = 80,
                        yfact = 30,
                        ypiket = 0,
                        textwidth=200,
                        XBase = 2000,
                        YBase = 1000;

                foreach (RowPiket R in F.data2)
                {
                    Line hLine = new Line(new Point3d(i * F.X+XBase, ygraf+YBase, 0), new Point3d(i * F.X + XBase, R.Project * F.Y+F.H+ygraf + YBase, 0));
                    hLine.Layer = mainLayerName;                    
                    acBlkTblRec.AppendEntity(hLine);
                    acTrans.AddNewlyCreatedDBObject(hLine, true);

                    hLine = new Line(new Point3d(i * F.X + XBase, yfact + YBase, 0), new Point3d(i * F.X + XBase, yfact-10 + YBase, 0));
                    hLine.Layer = mainLayerName;
                    acBlkTblRec.AppendEntity(hLine);
                    acTrans.AddNewlyCreatedDBObject(hLine, true);

                    DBText projectText = new DBText();
                    projectText.SetDatabaseDefaults();
                    projectText.Position = new Point3d(i * F.X+ XBase+3, yproject+5 + YBase, 0);
                    projectText.Height = 5;
                    projectText.TextString = R.Project.ToString();
                    projectText.Rotation = Math.PI/2;
                    projectText.Layer = progectLayerName;
                    acBlkTblRec.AppendEntity(projectText);
                    acTrans.AddNewlyCreatedDBObject(projectText, true);

                    DBText factText = new DBText();
                    factText.SetDatabaseDefaults();
                    factText.Position = new Point3d(i * F.X + XBase+3, yfact+5 + YBase, 0);
                    factText.Height = 5;
                    factText.TextString = R.Fact.ToString();
                    factText.Rotation = Math.PI / 2;
                    factText.Layer = factLayerName;
                    acBlkTblRec.AppendEntity(factText);
                    acTrans.AddNewlyCreatedDBObject(factText, true);

                    DBText deltaText = new DBText();
                    deltaText.SetDatabaseDefaults();
                    deltaText.Position = new Point3d(i * F.X + XBase+3, ydelta+5 + YBase, 0);
                    deltaText.Height = 5;
                    deltaText.TextString = R.Delta.ToString("N3");
                    deltaText.Rotation = Math.PI / 2;
                    deltaText.Layer = mainLayerName;
                    acBlkTblRec.AppendEntity(deltaText);
                    acTrans.AddNewlyCreatedDBObject(deltaText, true);

                    DBText piketText = new DBText();
                    piketText.SetDatabaseDefaults();
                    piketText.Position = new Point3d(i * F.X + XBase+F.X/2-5, yfact-10 + YBase, 0);
                    piketText.Height = 5;
                    piketText.TextString = F.X.ToString();
                    piketText.Layer = mainLayerName;
                    acBlkTblRec.AppendEntity(piketText);
                    acTrans.AddNewlyCreatedDBObject(piketText, true);


                    if (!int.TryParse(R.Piket, out int ip))
                    {
                        DBText piketsText = new DBText();
                        piketsText.SetDatabaseDefaults();
                        piketsText.Position = new Point3d(i * F.X + XBase + 3, ypiket - 20 + YBase, 0);
                        piketsText.Height = 5;
                        piketsText.TextString = R.Piket;
                        piketsText.Rotation = Math.PI / 2;
                        piketsText.Layer = mainLayerName;
                        acBlkTblRec.AppendEntity(piketsText);
                        acTrans.AddNewlyCreatedDBObject(piketsText, true);

                        hLine = new Line(new Point3d(i * F.X + XBase, yfact + YBase, 0), new Point3d(i * F.X + XBase, ypiket + YBase, 0));
                        hLine.Layer = mainLayerName;
                        acBlkTblRec.AppendEntity(hLine);
                        acTrans.AddNewlyCreatedDBObject(hLine, true);
                    }

                    projectPline.AddVertexAt(i, new Point2d(i * F.X + XBase, R.Project * F.Y + F.H + ygraf + YBase), 0, 0, 0);
                    factPline.AddVertexAt(i, new Point2d(i * F.X + XBase, R.Fact * F.Y + F.H + ygraf + YBase), 0, 0, 0);
                    i++;
                }

                i--;

                Line bLine = new Line(new Point3d(XBase-textwidth, yfact+YBase, 0), new Point3d(i * F.X + XBase, yfact + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                DBText TEXT1 = new DBText();
                TEXT1.SetDatabaseDefaults();
                TEXT1.Position = new Point3d(XBase + 15-textwidth, yfact + YBase+10, 0);
                TEXT1.Height = 10;
                TEXT1.TextString = "Фактические отметки";
                TEXT1.Layer = factLayerName;
                acBlkTblRec.AppendEntity(TEXT1);
                acTrans.AddNewlyCreatedDBObject(TEXT1, true);

                bLine = new Line(new Point3d(XBase - textwidth, yproject + YBase, 0), new Point3d(i * F.X + XBase, yproject + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                TEXT1 = new DBText();
                TEXT1.SetDatabaseDefaults();
                TEXT1.Position = new Point3d(XBase+15-textwidth, yproject + YBase+10, 0);
                TEXT1.Height = 10;
                TEXT1.TextString = "Проектные отметки";
                TEXT1.Layer = progectLayerName;
                acBlkTblRec.AppendEntity(TEXT1);
                acTrans.AddNewlyCreatedDBObject(TEXT1, true);

                bLine = new Line(new Point3d(XBase - textwidth, ydelta + YBase, 0), new Point3d(i * F.X + XBase, ydelta + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                TEXT1 = new DBText();
                TEXT1.SetDatabaseDefaults();
                TEXT1.Position = new Point3d(XBase + 15 - textwidth, ydelta + YBase + 10, 0);
                TEXT1.Height = 10;
                TEXT1.TextString = "Отклонения";
                TEXT1.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(TEXT1);
                acTrans.AddNewlyCreatedDBObject(TEXT1, true);

                bLine = new Line(new Point3d(XBase - textwidth, ypiket + YBase, 0), new Point3d(i * F.X + XBase, ypiket + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                TEXT1 = new DBText();
                TEXT1.SetDatabaseDefaults();
                TEXT1.Position = new Point3d(XBase + 15 - textwidth, ypiket + YBase + 10, 0);
                TEXT1.Height = 10;
                TEXT1.TextString = "Расстояния";
                TEXT1.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(TEXT1);
                acTrans.AddNewlyCreatedDBObject(TEXT1, true);

                bLine = new Line(new Point3d(XBase - textwidth, ygraf + YBase, 0), new Point3d(i * F.X + XBase, ygraf + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                bLine = new Line(new Point3d(XBase-F.X, ypiket + YBase, 0), new Point3d(XBase-F.X, ygraf + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);

                bLine = new Line(new Point3d(XBase - textwidth, ypiket + YBase, 0), new Point3d(XBase - textwidth, ygraf + YBase, 0));
                bLine.Layer = mainLayerName;
                acBlkTblRec.AppendEntity(bLine);
                acTrans.AddNewlyCreatedDBObject(bLine, true);


                // Добавление нового объекта в запись таблицы блоков и в транзакцию
                projectPline.Layer = progectLayerName;
                factPline.Layer = factLayerName;
              
                acBlkTblRec.AppendEntity(projectPline);
                acTrans.AddNewlyCreatedDBObject(projectPline, true);

                acBlkTblRec.AppendEntity(factPline);
                acTrans.AddNewlyCreatedDBObject(factPline, true);

                // Сохранение нового объекта в базе данных
                acTrans.Commit();
            }
        }

        // Функции Initialize() и Terminate() необходимы, чтобы реализовать интерфейс IExtensionApplication
        public void Initialize()
        {
           
        }

        public void Terminate()
        {

        }
    }


}
