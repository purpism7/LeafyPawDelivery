using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using System.Linq;

namespace Game
{
    public class TutorialManager : MonoBehaviour, Popo.IListener, UI.PopoConversation.IListener
    {
        private const float PopoPosY = -200f;
        private const float PopoPosZ = -10f;
        private const int StarterId = 1;

        public interface IListener
        {
            //void EndTutorial();

            void State(Game.Type.ETutorialStep eTutorialStep);
        }

        private IEvent _iEvent = null;
        private Game.Popo _popo = null;
        private UI.PopoConversation _popoConversation = null;

        public Game.Type.ETutorialStep ETutorialStep { get; private set; } = Game.Type.ETutorialStep.None;

        private List<IListener> _iListenerList = null;

        private void Update()
        {
            if (_popo != null)
            {
                _popo.ChainUpdate();
            }
        }

        public void Initialize(Transform rootTm)
        {
            AnimalManager.Event?.AddListener(OnChangedAnimal);
            ObjectManager.Event?.AddListener(OnChangedObject);

            if(CheckGetStarter)
            {
                ProcessAlreadyGetStater();
            }
            else
            {
                CreatePopo(rootTm);
            }
        }

        public void AddListener(IListener iListener)
        {
            if (iListener == null)
                return;

            if(_iListenerList == null)
            {
                _iListenerList = new();
                _iListenerList.Clear();
            }
            
            for (int i = 0; i < _iListenerList.Count; ++i)
            {
                if (_iListenerList[i] == null)
                    continue;

                if (_iListenerList[i].GetType().Equals(iListener.GetType()))
                    return;
            }

            _iListenerList.Add(iListener);
        }

        private void InvokeListener()
        {
            if (_iListenerList == null)
                return;

            foreach(var iListener in _iListenerList)
            {
                iListener?.State(ETutorialStep);
            }
        }

        private void CreatePopo(Transform rootTm)
        {
            _popo = GameSystem.ResourceManager.Instance?.InstantiateGame<Game.Popo>(rootTm);
            if (_popo == null)
                return;

            ProcessAppearPopo();
        }

        private void CreatePopoConversation()
        {
            _popoConversation = new GameSystem.PopupCreator<UI.PopoConversation, UI.PopoConversation.Data>()
               .SetShowBackground(false)
               .SetAnimActivate(false)
               .SetData(new UI.PopoConversation.Data()
               {
                   iListener = this,
               })
               .SetForTutorial(true)
               .Create();

            _popoConversation?.Deactivate();
        }

        private async UniTask StartSpeechBubbleAsync(string sentence, float keepDelay, float delay = 0)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(delay));

            _popo?.StartSpeechBubble(sentence, keepDelay);
        }

        private void Process()
        {
            switch (ETutorialStep)
            {
                case Game.Type.ETutorialStep.HiPopo:
                    {
                        ProcessHiPopo();

                        break;
                    }

                case Game.Type.ETutorialStep.DescGame:
                    {
                        ProcessDescGame();

                        break;
                    }

                case Game.Type.ETutorialStep.Start:
                    {
                        ProcessStartDescAsync().Forget();

                        break;
                    }

                case Game.Type.ETutorialStep.DescAnimalCurrency:
                    {
                        ProcessDescAnimalCurrency();

                        break;
                    }

                case Game.Type.ETutorialStep.DescObjectCurrency:
                    {
                        ProcessDescObjectCurrnecy();

                        break;
                    }

                case Game.Type.ETutorialStep.DescLetter:
                    {
                        ProcessDescLetter();

                        break;
                    }

                case Game.Type.ETutorialStep.GetStarter:
                    {
                        ProcessGetStarterAsync().Forget();

                        break;
                    }

                case Game.Type.ETutorialStep.DescEdit:
                    {
                        ProcessDescEditAsync(0.7f).Forget();

                        break;
                    }

                case Game.Type.ETutorialStep.DisappearPopo:
                    {
                        ProcessDisappearPopoAsync().Forget();

                        break;
                    }

                case Game.Type.ETutorialStep.DescMap:
                    {
                        ProcessDescMap();

                        break;
                    }

                case Game.Type.ETutorialStep.HappyLeafyPawDelivery:
                    {
                        ProcessHappyLeafyPawDeliveryAsync().Forget();

                        break;
                    }
            }
        }

        private void SetStep(Game.Type.ETutorialStep step)
        {
            ETutorialStep = step;

            if (step == Type.ETutorialStep.None)
                return;

            InvokeListener();
        }

        private void ProcessAppearPopo()
        {
            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return;

            _popo.Initialize(new Popo.Data()
            {
                iListener = this,
                startPos = new Vector3(-iGameCameraCtr.GameCameraWidth + 500f, PopoPosY, PopoPosZ),
            });

            _popo?.MoveToTarget(new Vector3(0, PopoPosY, PopoPosZ));

            SetStep(Game.Type.ETutorialStep.HiPopo);
        }

        private void ProcessHiPopo()
        {
            StartSpeechBubbleAsync("안녕 난 포포야!", 3f, 0.5f).Forget();

            SetStep(Game.Type.ETutorialStep.DescGame);
        }

        private async UniTask ProcessStartDescAsync()
        {
            StartSpeechBubbleAsync("이 게임에 대해서 설명해줄게.", 3f).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

            ActivateAnimTop();

            SetStep(Game.Type.ETutorialStep.DescAnimalCurrency);
        }

        private void ProcessDescGame()
        {
            StartSpeechBubbleAsync("\"숲속의 우체부\"는 재화를 모아서 \n마을을 꾸미는 게임이야.", 4f).Forget();

            SetStep(Game.Type.ETutorialStep.Start);
        }

        private void ProcessDescAnimalCurrency()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateGuideLine(Game.Type.ECategory.AnimalCurrency);

            CreatePopoConversation();

            _popoConversation?.ActivateBottom("\"주민 재화\"는 배치된 주민들이 일정 시간마다 떨어트려. \n터치해서 주우면 획득할 수 있어.");
            _popoConversation?.Activate();

            SetStep(Game.Type.ETutorialStep.DescObjectCurrency);
        }

        private void ProcessDescObjectCurrnecy()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateGuideLine(Game.Type.ECategory.ObjectCurrency);

            _popoConversation?.ActivateBottom("\"꾸미기 재화\"는 배치된 꾸미기 요소들을 터치하면 획득할 수 있어.");

            SetStep(Game.Type.ETutorialStep.DescLetter);
        }

        private void ProcessDescLetter()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateLetterGuideLine();

            _popoConversation?.ActivateBottom("마을 바닥에 \"떨어진 편지\"를 연타하면 다양한 재화를 얻을 수 있어.");

            SetStep(Game.Type.ETutorialStep.GetStarter);
        }

        private async UniTask ProcessGetStarterAsync()
        {
            bool isAnimalStarter = false;
            bool isObjectStarter = false;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr != null)
            {
                isAnimalStarter = !mainGameMgr.CheckExist(Type.EElement.Animal, StarterId);
                isObjectStarter = !mainGameMgr.CheckExist(Type.EElement.Object, StarterId);
            }

            _popoConversation?.Deactivate();

            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.AllDeactivateGuideLine();

            if (isAnimalStarter || isObjectStarter)
            {
                StartSpeechBubbleAsync("잘 따라오고 있어! 그런 의미로 이건 내가 주는 선물이야.", 3f).Forget();

                await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
            }

            bool existStarter = false;
            if (mainGameMgr != null)
            {
                if (isAnimalStarter)
                {
                    var iEvent = MainGameManager.Get<AnimalManager>() as IEvent;
                    iEvent?.Starter(null);
                }
                else
                {
                    existStarter = true;
                }

                if (isObjectStarter)
                {
                    var iEvent = MainGameManager.Get<ObjectManager>() as IEvent;
                    iEvent?.Starter(
                        () =>
                        {
                            SetStep(Game.Type.ETutorialStep.DescEdit);

                            Process();
                        });
                }
                else
                {
                    existStarter = true;
                }
            }

            if (existStarter)
            {
                SetStep(Game.Type.ETutorialStep.DescEdit);

                Process();
            }
            else
            {
                SetStep(Game.Type.ETutorialStep.None);
            }
        }

        private async UniTask ProcessDescEditAsync(float delay)
        {
            StartSpeechBubbleAsync("우리 이제 배치를 한 번 해볼까?", 3f).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(delay));

            var uiMgr = UIManager.Instance;
            if (uiMgr == null)
                return;

            var eBottomTypes = new Type.EBottomType[] { Type.EBottomType.Arrangement };

            uiMgr.SetInteractable(false, eBottomTypes);

            var bottom = uiMgr.Bottom;
            bottom?.ActivateAnim(
                () =>
                {
                    bottom?.ActivateGuideLine(eBottomTypes);
                });

            SetStep(Game.Type.ETutorialStep.DisappearPopo);
        }

        private async UniTask ProcessDisappearPopoAsync()
        {
            StartSpeechBubbleAsync("휴우~ 배치하는 동안 난 잠깐 쉬고 있을게.", 3f).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.7f));

            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return;

            _popo?.MoveToTarget(new Vector3(iGameCameraCtr.GameCameraWidth, PopoPosY, PopoPosZ));
        }

        // 이미 스타터팩을 지급 받고 중간에 튜토리얼 진행 시.
        private void ProcessAlreadyGetStater()
        {
            ActivateAnimTop();

            CreatePopoConversation();

            _popoConversation?.ActivateTop("우리 이제 배치를 한 번 해볼까?", true);
            _popoConversation?.Activate();

            ProcessDescEditAsync(0).Forget();

            //StopUpdateGameCamera(false);

            SetStep(Game.Type.ETutorialStep.EditAnimal);
        }

        private void ProcessEditObject()
        {
            _popoConversation?.ActivateTop("잘했어! 우리 이번엔 \"커다란 나무\"를 한 번 배치해 볼까?", true);
            _popoConversation?.Activate();

            SetStep(Type.ETutorialStep.EditObject);
        }

        private async UniTask ProcessDescStory()
        {
            _popoConversation?.ActivateCenter("꾸미기 요소를 획득하다보면, 새로운 스토리도 열릴거야.");

            var uiMgr = UIManager.Instance;
            if (uiMgr == null)
                return;

            uiMgr.SetInteractable(false);

            var bottom = uiMgr.Bottom;
            bottom?.AllDeactivateGuideLine();

            await UniTask.WaitForSeconds(0.5f);

            bottom?.DeactivateEditList();

            await UniTask.WaitForSeconds(0.7f);

            SetStep(Type.ETutorialStep.DescMap);
        }

        private void ProcessDescMap()
        {
            _popoConversation?.ActivateCenter("모든 주민과 꾸미기 요소를 획득하면 다음 마을이 열릴거야.");

            SetStep(Type.ETutorialStep.HappyLeafyPawDelivery);
        }

        private async UniTask ProcessHappyLeafyPawDeliveryAsync()
        {
            _popoConversation?.ActivateCenter("여기까지야! \"숲속의 우체부\"를 재밌게 플레이 해줘.");

            Info.Connector.Get?.SetCompleteTutorial(true);

            SetStep(Type.ETutorialStep.None);

            await UniTask.WaitForSeconds(3f);

            _popoConversation?.Deactivate();

            SetStep(Type.ETutorialStep.End);

            await UniTask.WaitForSeconds(1f);

            SetStep(Type.ETutorialStep.ReturnGame);

            UIManager.Instance?.SetInteractable(true);

            //_popoConversation?.Deactivate();
        }

        private void ActivateAnimTop()
        {
            var uiMgr = UIManager.Instance;
            if (uiMgr == null)
                return;

            uiMgr.SetInteractable(false);

            var iTop = uiMgr.Top;
            iTop?.ActivateAnim(() =>
            {

            });
        }

        private bool CheckGetStarter
        {
            get
            {
                var mainGameMgr = MainGameManager.Instance;
                if (mainGameMgr != null)
                {
                    return mainGameMgr.CheckExist(Type.EElement.Animal, StarterId) || mainGameMgr.CheckExist(Type.EElement.Object, StarterId);
                }

                return false;
            }
        }

        #region Popo.IListener
        void Popo.IListener.EndMove()
        {
            if (ETutorialStep == Game.Type.ETutorialStep.DisappearPopo)
            {
                _popo?.Deactivate();
                //StopUpdateGameCamera(false);

                SetStep(Game.Type.ETutorialStep.EditAnimal);
            }

            Process();
        }

        void Popo.IListener.EndSpeechBubble()
        {
            Process();
        }
        #endregion

        #region PopoConversation.IListener
        void UI.PopoConversation.IListener.Click()
        {
            Process();
        }
        #endregion

        private void OnChangedAnimal(Game.Event.AnimalData animalData)
        {
            switch (animalData)
            {
                case Game.Event.ArrangeAnimalData arrangeAnimalData:
                    {
                        if (arrangeAnimalData.id == StarterId)
                        {
                            ProcessEditObject();
                        }

                        break;
                    }
            }
        }

        private void OnChangedObject(Game.Event.ObjectData objectData)
        {
            switch (objectData)
            {
                case Game.Event.ArrangeObjectData arrangeObjectData:
                    {
                        if (arrangeObjectData.id == StarterId)
                        {
                            ProcessDescStory().Forget();
                        }

                        break;
                    }
            }

        }
    }
}
