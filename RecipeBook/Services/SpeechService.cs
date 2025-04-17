using Microsoft.Maui.Controls;
using RecipeBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeBook.Services
{
    public class SpeechService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSpeaking = false;

        public bool IsSpeaking => _isSpeaking;


        public void StopSpeaking()
        {
            if (_isSpeaking)
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                _isSpeaking = false;
            }
        }

        public async Task SpeakRecipeStepsAsync(List<RecipeBook.Models.RecipeStep> steps, float volume = 1.0f, float pitch = 1.0f, float rate = 1.0f)
        {
            if (steps == null || steps.Count == 0)
                return;

            // Cancel any ongoing speech
            StopSpeaking();

            _cancellationTokenSource = new CancellationTokenSource();
            _isSpeaking = true;

            try
            {
                // Get all available locales
                IEnumerable<Locale> availableLocales = await TextToSpeech.GetLocalesAsync();


                var speechOptions = new SpeechOptions
                {
                    Volume = volume,
                    Pitch = pitch,
                };

                for (int i = 0; i < steps.Count; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    string stepText = $"Step {i + 1}. {steps[i].Text}";
                    await TextToSpeech.SpeakAsync(stepText, speechOptions, _cancellationTokenSource.Token);

                    // Pause briefly between steps
                    await Task.Delay(1000, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Speech was canceled, do nothing
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Speech error: {ex.Message}");
            }
            finally
            {
                _isSpeaking = false;
            }
        }

    }
}