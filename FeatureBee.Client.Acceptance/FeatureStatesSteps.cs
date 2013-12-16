﻿using System.Collections.Generic;
using FeatureBee.Client;
using FeatureBee.Client.Evaluators;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FeatureBee.Acceptance
{
    [Binding]
    public class FeatureStatesSteps
    {
        private bool _featureIsEnabled;
        private readonly Mock<IFeatureRepository> _featureRepositoryMock = new Mock<IFeatureRepository>();
        private readonly Mock<IConditionEvaluator> _conditionEvaluatorsMock = new Mock<IConditionEvaluator>();
        private readonly Mock<IConditionEvaluator> _conditionEvaluatorsMock2 = new Mock<IConditionEvaluator>();

        [Given(@"I have feature with a condition evaluator that is always fullfilled")]
        public void GivenIHaveFeatureWithAConditionEvaluatorThatIsAlwaysFullfilled()
        {
            _conditionEvaluatorsMock.Setup(x => x.Name).Returns("FakeEvaluator");
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<object>())).Returns(true);

            FeatureBeeConfig
                .Init()
                .UsingEvaluators(new List<IConditionEvaluator> { _conditionEvaluatorsMock.Object })
                .UsingRepository(_featureRepositoryMock.Object)
                .Build();
        }

        [Given(@"I have feature under test with a condition evaluator")]
        public void GivenIHaveFeatureUnderTestWithAConditionEvaluator()
        {
            var conditions = new List<Condition> { new Condition { Evaluator = "FakeEvaluator", Value = null } };
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<Feature>() { new Feature { Name = "SampleFeature", State = "Under Test", Conditions = conditions } });
            _conditionEvaluatorsMock.Setup(x => x.Name).Returns("FakeEvaluator");

            FeatureBeeConfig
                .Init()
                .UsingEvaluators(new List<IConditionEvaluator> { _conditionEvaluatorsMock.Object })
                .UsingRepository(_featureRepositoryMock.Object)
                .Build();
        }

        [Given(@"the condition is (.*)")]
        public void GivenTheConditionIs(bool condition)
        {
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<object>())).Returns(condition);
        }

        [Given(@"I have a feature in state (.*)")]
        public void GivenIHaveAFeatureInState(string featureState)
        {
            var conditions = new List<Condition> { new Condition { Evaluator = "FakeEvaluator", Value = null } };
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<Feature>() { new Feature { Name = "SampleFeature", State = featureState, Conditions = conditions } });
        }

        [Given(@"the feature has a second condition evaluator")]
        public void GivenTheFeatureHasASecondConditionEvaluator()
        {
            var conditions = new List<Condition>
            {
                new Condition { Evaluator = "FakeEvaluator", Value = null },
                new Condition { Evaluator = "FakeEvaluator2", Value = null }
            };
            _featureRepositoryMock.Setup(x => x.GetFeatures())
                .Returns(new List<Feature>()
                {
                    new Feature {Name = "SampleFeature", State = "Under Test", Conditions = conditions}
                });
            _conditionEvaluatorsMock2.Setup(x => x.Name).Returns("FakeEvaluator2");

            FeatureBeeConfig
                .Init()
                .UsingEvaluators(new List<IConditionEvaluator> { _conditionEvaluatorsMock.Object, _conditionEvaluatorsMock2.Object })
                .UsingRepository(_featureRepositoryMock.Object)
                .Build();
        }

        [Given(@"the first condition evaluator returns (.*)")]
        public void GivenTheFirstConditionEvaluatorReturns(bool firstEvaluator)
        {
            _conditionEvaluatorsMock.Setup(x => x.IsFulfilled(It.IsAny<object>())).Returns(firstEvaluator);
        }

        [Given(@"the second condition evaluator returns (.*)")]
        public void GivenTheSecondConditionEvaluatorReturns(bool secondEvaluator)
        {
            _conditionEvaluatorsMock2.Setup(x => x.IsFulfilled(It.IsAny<object>())).Returns(secondEvaluator);
        }

        [When(@"evaluating the feature state")]
        public void WhenEvaluatingTheFeatureState()
        {
            _featureIsEnabled = Client.FeatureBee.IsFeatureEnabled("SampleFeature");
        }

        [Then(@"the (.*)")]
        public void ThenTheFeatureIsEnabled(bool featureIsEnabled)
        {
            Assert.AreEqual(featureIsEnabled, _featureIsEnabled);
        }

        [Then(@"(.*) to evaluate the state")]
        public void ThenToEvaluateTheState(bool conditionsWhereUsed)
        {
            if (conditionsWhereUsed)
                _conditionEvaluatorsMock.Verify(x => x.IsFulfilled(It.IsAny<object>()), Times.Once);
            else
             _conditionEvaluatorsMock.Verify(x => x.IsFulfilled(It.IsAny<object>()), Times.Never);
        }
    }
}