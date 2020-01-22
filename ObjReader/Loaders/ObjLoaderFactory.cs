﻿using ObjReader.Data.DataStore;
using ObjReader.TypeParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjReader.Loaders {

    public class ObjLoaderFactory : IObjLoaderFactory {

        public static IObjLoader Create() {

            IObjLoaderFactory f = new ObjLoaderFactory();
            return f.Create(new MaterialStreamProvider());
        }

        IObjLoader IObjLoaderFactory.Create() {

            return (this as IObjLoaderFactory).Create(new MaterialStreamProvider());
        }

        IObjLoader IObjLoaderFactory.Create(IMaterialStreamProvider materialStreamProvider) {

            var dataStore = new DataStore();

            var faceParser = new FaceParser(dataStore);
            var groupParser = new GroupParser(dataStore);
            var objectParser = new ObjectParser(dataStore);
            var normalParser = new NormalParser(dataStore);
            var textureParser = new TextureParser(dataStore);
            var vertexParser = new VertexParser(dataStore);

            var materialLibraryLoader = new MaterialLibraryLoader(dataStore);
            var materialLibraryLoaderFacade = new MaterialLibraryLoaderFacade(materialLibraryLoader, materialStreamProvider);
            var materialLibraryParser = new MaterialLibraryParser(materialLibraryLoaderFacade);
            var useMaterialParser = new UseMaterialParser(dataStore);

            return new ObjLoader(dataStore, faceParser, groupParser, objectParser, normalParser, textureParser, vertexParser, materialLibraryParser, useMaterialParser);
        }
    }
}
