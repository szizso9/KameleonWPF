using Kameleon2.Model;
using Kameleon2.Persistence;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Data;
using Castle.Components.DictionaryAdapter.Xml;
using System.Drawing;
using System.Reflection;

namespace Kameleon2.Kameleon2Test
{
    [TestClass]
    public class KameleonGameModelTest
    {
        private KameleonGameModel _model;
        private KameleonMap _mockedMap;
        private Mock<IKameleonDataAccess> _mock;


        [TestInitialize]
        public void Initialize()
        {
            _mockedMap = new KameleonMap(3);
            _mockedMap.setFieldsColor(0, 0, Persistence.Color.Red);
            _mockedMap.setFieldsColor(0, 1, Persistence.Color.Red);
            _mockedMap.setFieldsColor(0, 2, Persistence.Color.Red);
            _mockedMap.setFieldsColor(1, 0, Persistence.Color.Red);
            _mockedMap.setFieldsColor(1, 1, Persistence.Color.Empty);
            _mockedMap.setFieldsColor(1, 2, Persistence.Color.Green);
            _mockedMap.setFieldsColor(2, 0, Persistence.Color.Green);
            _mockedMap.setFieldsColor(2, 1, Persistence.Color.Green);
            _mockedMap.setFieldsColor(2, 2, Persistence.Color.Green);

            _mock = new Mock<IKameleonDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>())).Returns(() => Task.FromResult(_mockedMap));

            _model = new KameleonGameModel(_mock.Object);

            _model.GameAdvanced += new EventHandler<KameleonEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<KameleonEventArgs>(Model_GameOver);




        }

        private void Model_GameOver(object? sender, KameleonEventArgs e)
        {
            Assert.IsTrue(_model.Map.isOver());
            Assert.AreEqual(_model.Player, e.Player);
            Assert.AreEqual(true, e.IsWon);
        }

        private void Model_GameAdvanced(object? sender, KameleonEventArgs e)
        {
            Assert.IsFalse(_model.Map.isOver());
            Assert.AreEqual(_model.Player, e.Player);
            Assert.AreEqual(false, e.IsWon);

        }

        [TestMethod]
        public void KameleonGameModelNewGameMediumMapTest()
        {
            _model.fNewGame();
            Assert.AreEqual(5, _model.Map.MapSize);
            int green = 0;
            int red = 0;
            int empty = 0;

            for (int i = 0; i < _model.Map.MapSize; i++)
            {
                for (int j = 0; j < _model.Map.MapSize; j++)
                {
                    if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Red) red++;
                    else if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Green) green++;
                    else empty++;
                }
            }
            Assert.AreEqual(1, empty);
            Assert.AreEqual(12, red);
            Assert.AreEqual(12, green);

        }

        [TestMethod]
        public void KameleonGameModelNewGameLittleMapTest()
        {
            _model.fMapType = Model.MapType.Little;
            _model.fNewGame();
            Assert.AreEqual(3, _model.Map.MapSize);
            int green = 0;
            int red = 0;
            int empty = 0;

            for (int i = 0; i < _model.Map.MapSize; i++)
            {
                for (int j = 0; j < _model.Map.MapSize; j++)
                {
                    if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Red) red++;
                    else if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Green) green++;
                    else empty++;
                }
            }
            Assert.AreEqual(1, empty);
            Assert.AreEqual(4, red);
            Assert.AreEqual(4, green);

        }

        [TestMethod]
        public void KameleonGameModelNewGameLargeMapTest()
        {
            _model.fMapType = Model.MapType.Large;
            _model.fNewGame();
            Assert.AreEqual(7, _model.Map.MapSize);
            int green = 0;
            int red = 0;
            int empty = 0;

            for (int i = 0; i < _model.Map.MapSize; i++)
            {
                for (int j = 0; j < _model.Map.MapSize; j++)
                {
                    if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Red) red++;
                    else if (_model.Map.getFieldsPlayer(i, j) == Persistence.Color.Green) green++;
                    else empty++;
                }
            }
            Assert.AreEqual(1, empty);
            Assert.AreEqual(24, red);
            Assert.AreEqual(24, green);

        }

        [TestMethod]
        public void KameleonGameModelStepTest()
        {
            Point a = new Point(0, 1);
            Point b = new Point(2, 1);

            _model.fMapType = Model.MapType.Little;
            _model.fNewGame();

            Assert.AreEqual(Player.Green, _model.Player);

            _model.Step(a, b);
            Assert.AreEqual(Persistence.Color.Red, _model.Map.getFieldsPlayer(a.X, a.Y));
            Assert.AreEqual(Persistence.Color.Green, _model.Map.getFieldsPlayer(b.X, b.Y));

            a.X = 2; a.Y = 1;
            b.X = 1; b.Y = 1;

            _model.Step(a, b);
            Assert.AreEqual(Persistence.Color.Empty, _model.Map.getFieldsPlayer(a.X, a.Y));
            Assert.AreEqual(Persistence.Color.Green, _model.Map.getFieldsPlayer(b.X, b.Y));

            _model.Advance();

            a.X = 0; a.Y = 1;
            b.X = 2; b.Y = 1;

            _model.Step(a, b);
            Assert.AreEqual(Persistence.Color.Empty, _model.Map.getFieldsPlayer(a.X, a.Y));
            Assert.AreEqual(Persistence.Color.Red, _model.Map.getFieldsPlayer(b.X, b.Y));
            Assert.AreEqual(Persistence.Color.Empty, _model.Map.getFieldsPlayer(1, 1));



        }

        [TestMethod]
        public void KameleonGameModelAdvanceTest()
        {
            _model.fMapType = MapType.Medium;
            _model.fNewGame();
            Assert.AreEqual(Player.Green, _model.Player);
            _model.Advance();
            Assert.AreEqual(Player.Red, _model.Player);
        }

        [TestMethod]
        public async Task KameleonGameModelLoadTest()
        {

            _model.fNewGame();


            await _model.LoadGameAsync(string.Empty);

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Assert.AreEqual(_mockedMap.getFieldsColor(i, j), _model.Map.getFieldsColor(i, j));
                    Assert.AreEqual(_mockedMap.getFieldsPlayer(i, j), _model.Map.getFieldsPlayer(i, j));
                }
            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }


    }
}