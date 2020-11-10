﻿//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using NumSharp;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Daany;
using Microsoft.ML;
using Daany.Ext;


namespace Anndotnet.Core.Extensions
{
    public static class DFExtensions
    {
       public static (NDArray X, NDArray Y)  TransformData(this DataFrame df, List<ColumnInfo> metadata)
        {
            //extract features and label from DataFrame
            var feats = metadata.Where(x=>x.MLType == MLColumnType.Feature).ToList();
            var labelInfo = metadata.Where(x => x.MLType == MLColumnType.Label).ToList();

            //transform feature
            var dfF = df[feats.Select(x => x.Name).ToArray()];
            var featureDf = prepareDf(dfF, feats);

            
            //transform label
            var lDf = df.Create((labelInfo.Select(x=>x.Name).FirstOrDefault(), null));
            var labelDf = prepareDf(lDf, labelInfo);
            
            //iterate through rows
            var x = featureDf.ToNDArray();
            var y = labelDf.ToNDArray();
            //
            return (x, y);
        }

       private static DataFrame prepareDf(DataFrame df, List<ColumnInfo> metadata)
        {
            var cols = df.Columns;

            //check id all columns have valid type
            if (df.ColTypes.Any(x => x == ColType.DT))
                throw new Exception("DataTime column cannot be directly prepare to ML. Consider to transform it to another type.");

            //string and categorical column should be transformed in to OneHot Encoding
            var finalColumns = new List<String>();
            var finalDf = df[df.Columns.ToArray()];

            for (int j = 0; j < df.ColCount(); j++)
            {
                //categorical data encoding
                if (metadata[j].Transformer.DataNormalization == ColumnTransformer.Binary1 ||
                    metadata[j].Transformer.DataNormalization == ColumnTransformer.Binary2 ||
                    metadata[j].Transformer.DataNormalization == ColumnTransformer.Dummy ||
                    metadata[j].Transformer.DataNormalization == ColumnTransformer.OneHot ||
                    metadata[j].Transformer.DataNormalization == ColumnTransformer.Ordinal)
                {
                    (var edf, var vVal, var eVal) = df.TransformColumn(cols[j], metadata[j].Transformer.DataNormalization, true);
                    finalDf = finalDf.Append(edf, verticaly: false);
                    //store encoded class values to metadata
                    metadata[j].Transformer.ClassValues = eVal;

                    //add to column list
                    if (edf == null)
                        continue;

                    foreach (var c in edf.Columns)
                    {
                        finalColumns.Add(c);
                    }

                }
                //Data normalization or scaling
                else if (metadata[j].Transformer.DataNormalization == ColumnTransformer.MinMax ||
                        metadata[j].Transformer.DataNormalization == ColumnTransformer.Standardizer)
                {
                    (var ndf, var nVal, var sVal) = df.TransformColumn(cols[j], metadata[j].Transformer.DataNormalization, true);
                    metadata[j].Transformer.NormalizationValues = nVal;
                    finalColumns.Add(cols[j]);
                }
                else
                 finalColumns.Add(cols[j]);
            }

            return finalDf[finalColumns.ToArray()];
        }

       public static NDArray ToNDArray(this DataFrame df)
        {
            var shape = new Shape(df.RowCount(), df.ColCount());
            var lst = new List<float>();
            foreach(var r in df.GetRowEnumerator())
                lst.AddRange(r.Select(x=> Convert.ToSingle(x)).ToList());
            var arr = lst.ToArray();

            //
            var ndArray = new NDArray(arr);
            
            //reshape the data if the dimension is greather than 1
            if(df.ColCount()>0)
                ndArray = ndArray.reshape(shape);

            return ndArray;
        }

       public static List<ColumnInfo> ParseMetadata(this DataFrame df, string label)
        {
            List<ColumnInfo> cols = new List<ColumnInfo>();
            for(int i=0; i < df.ColCount(); i++)
            {
                var name = df.Columns[i];
                var type = df.ColTypes[i];

                var c = new ColumnInfo();
                if (name == label)
                {
                    c.MLType = MLColumnType.Label;
                    
                    if (type== ColType.IN || type == ColType.STR)
                    {
                        c.ValueColumnType = ColType.IN;
                        c.Transformer.DataNormalization = ColumnTransformer.OneHot;
                    }
                    else
                        c.ValueColumnType = type;
                }
                else
                {
                    c.MLType = MLColumnType.Feature;
                    c.ValueColumnType = type;
                    if (type == ColType.IN)
                        c.Transformer.DataNormalization = ColumnTransformer.Ordinal;
                }

                //
                c.Id = i;
                c.Transformer.ClassValues = null;
                c.MissingValue = Aggregation.None;
                c.Name = name;
                
                cols.Add(c);
            }

            return cols;
        }

       public static DataFrame HandlingMissingValue(this DataFrame df, List<ColumnInfo> metadata)
       {
           foreach(var m in metadata)                
            {
                df.FillNA(m.Name, m.MissingValue);
            }
           return df;
       }
    }
}
