using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using System.Linq;
using UnityEngine.Localization.Settings;

namespace Game
{
    public class TutorialManager : MonoBehaviour, Popo.IListener, UI.PopoConversation.IListener
    {
        private const float PopoPosY = -200f;
        private const float PopoPosZ = -10f;

        public interface IListener
        {
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

            _popo?.Initialize(
                new Popo.Data()
                {
                    iListener = this,
                    startPos = new Vector3(-iGameCameraCtr.GameCameraWidth + 800f, PopoPosY, PopoPosZ),
                });

            _popo?.MoveToTarget(new Vector3(0, PopoPosY, PopoPosZ));

            SetStep(Game.Type.ETutorialStep.HiPopo);
        }

        private void ProcessHiPopo()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_hi_popo", LocalizationSettings.SelectedLocale);

            StartSpeechBubbleAsync(local, 3f, 0.5f).Forget();

            SetStep(Game.Type.ETutorialStep.DescGame);
        }

        private async UniTask ProcessStartDescAsync()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_start_desc", LocalizationSettings.SelectedLocale);

            StartSpeechBubbleAsync(local, 3f).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

            ActivateAnimTop();

            SetStep(Game.Type.ETutorialStep.DescAnimalCurrency);
        }

        private void ProcessDescGame()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_game", LocalizationSettings.SelectedLocale);

            StartSpeechBubbleAsync(local, 4f).Forget();

            SetStep(Game.Type.ETutorialStep.Start);
        }

        private void ProcessDescAnimalCurrency()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateGuideLine(Game.Type.ECategory.AnimalCurrency);

            CreatePopoConversation();

            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_animal_currency", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateBottom(local);
            _popoConversation?.Activate();

            SetStep(Game.Type.ETutorialStep.DescObjectCurrency);
        }

        private void ProcessDescObjectCurrnecy()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateGuideLine(Game.Type.ECategory.ObjectCurrency);

            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_object_currency", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateBottom(local);

            SetStep(Game.Type.ETutorialStep.DescLetter);
        }

        private void ProcessDescLetter()
        {
            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.ActivateLetterGuideLine();

            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_letter", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateBottom(local);

            SetStep(Game.Type.ETutorialStep.GetStarter);
        }

        private async UniTask ProcessGetStarterAsync()
        {
            bool isAnimalStarter = false;
            bool isObjectStarter = false;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr != null)
            {
                isAnimalStarter = !mainGameMgr.CheckExist(Type.EElement.Animal, Games.Data.Const.TutorialAnimalId);
                isObjectStarter = !mainGameMgr.CheckExist(Type.EElement.Object, Games.Data.Const.TutorialObjectId);
            }

            _popoConversation?.Deactivate();

            UI.ITop iTop = UIManager.Instance?.Top;
            iTop?.AllDeactivateGuideLine();

            if (isAnimalStarter || isObjectStarter)
            {
                var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_get_starter", LocalizationSettings.SelectedLocale);

                StartSpeechBubbleAsync(local, 2f).Forget();

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
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_edit", LocalizationSettings.SelectedLocale);

            StartSpeechBubbleAsync(local, 2f).Forget();

            await UniTask.Delay(System.TimeSpan.FromSeconds(delay));

            SetStep(Game.Type.ETutorialStep.DisappearPopo);
        }

        private async UniTask ProcessDisappearPopoAsync()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_disappear_popo", LocalizationSettings.SelectedLocale);

            StartSpeechBubbleAsync(local, 2f).Forget();

            SetStep(Game.Type.ETutorialStep.DisappearPopoEndMove);

            await UniTask.Delay(System.TimeSpan.FromSeconds(1f));

            var iGameCameraCtr = MainGameManager.Instance?.IGameCameraCtr;
            if (iGameCameraCtr == null)
                return;

            _popo?.MoveToTarget(new Vector3(iGameCameraCtr.GameCameraWidth, PopoPosY, PopoPosZ));

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.7f));

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
        }

        // 이미 스타터팩을 지급 받고 중간에 튜토리얼 진행 시.
        private void ProcessAlreadyGetStater()
        {
            ActivateAnimTop();

            CreatePopoConversation();

            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_edit", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateTop(local, true);
            _popoConversation?.Activate();

            ProcessDescEditAsync(0).Forget();

            ProcessDisappearPopoAsync().Forget();
        }

        private void ProcessEditObject()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_edit_object", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateTop(local, true);
            _popoConversation?.Activate();
            _popo?.Deactivate();

            SetStep(Type.ETutorialStep.EditObject);
        }

        private async UniTask ProcessDescStory()
        {
            _popo?.Deactivate();

            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_story", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateCenter(local);

            var uiMgr = UIManager.Instance;
            if (uiMgr == null)
                return;

            uiMgr.SetInteractable(false);

            var bottom = uiMgr.Bottom;
            bottom?.AllDeactivateGuideLine();

            await UniTask.WaitForSeconds(1f);

            bottom?.DeactivateEditList();

            await UniTask.WaitForSeconds(0.7f);

            SetStep(Type.ETutorialStep.DescMap);
        }

        private void ProcessDescMap()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_desc_map", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateCenter(local);

            SetStep(Type.ETutorialStep.HappyLeafyPawDelivery);
        }

        private async UniTask ProcessHappyLeafyPawDeliveryAsync()
        {
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "tutorial_happy_leafypawdelivery", LocalizationSettings.SelectedLocale);

            _popoConversation?.ActivateCenter(local);

            Info.Connector.Get?.SetCompleteTutorial(true);

            SetStep(Type.ETutorialStep.None);

            await UniTask.WaitForSeconds(3f);

            _popoConversation?.Deactivate();

            SetStep(Type.ETutorialStep.End);

            await UniTask.WaitForSeconds(1f);

            SetStep(Type.ETutorialStep.ReturnGame);

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
                    return mainGameMgr.CheckExist(Type.EElement.Animal, Games.Data.Const.TutorialAnimalId) || mainGameMgr.CheckExist(Type.EElement.Object, Games.Data.Const.TutorialObjectId);
                }

                return false;
            }
        }

        #region Popo.IListener
        void Popo.IListener.EndMove()
        {
            if (ETutorialStep == Game.Type.ETutorialStep.DisappearPopoEndMove)
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
            if (ETutorialStep >= Type.ETutorialStep.EditObject)
                return;

            switch (animalData)
            {
                case Game.Event.ArrangeAnimalData arrangeAnimalData:
                    {
                        if (arrangeAnimalData.id == Games.Data.Const.TutorialAnimalId)
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
                        if (arrangeObjectData.id == Games.Data.Const.TutorialObjectId)
                        {
                            ProcessDescStory().Forget();
                        }

                        break;
                    }
            }
        }
    }
}

