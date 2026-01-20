using System;
using System.IO;
using System.Windows.Media;
using System.Windows;

namespace ChessUI
{
    public class AudioManager
    {
        private MediaPlayer _player;

        public AudioManager()
        {
            _player = new MediaPlayer();
        }
        public void PlayMove() => Play("move-self.mp3");
        public void PlayCapture() => Play("capture.mp3");
        public void PlayCastle() => Play("castle.mp3");
        public void PlayPromote() => Play("promote.mp3");

        public void PlayGameStart() => Play("game-start.mp3");
        public void PlayGameEnd() => Play("game-end.mp3");

        public void PlayPuzzleCorrect() => Play("puzzle-correct.mp3");
        public void PlayPuzzleWrong() => Play("puzzle-wrong.mp3");

        private void Play(string fileName)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Sounds", fileName);
                if (!File.Exists(path))
                {
                    MessageBox.Show($"Không tìm thấy file nhạc tại:\n{path}", "Lỗi Âm Thanh");
                    return;
                }
                if (File.Exists(path))
                {
                    _player.Open(new Uri(path));
                    _player.Volume = 1.0;
                    _player.Play();
                }
            }
            catch
            {
                MessageBox.Show("Error playing sound: " + fileName);
            }
        }
    }
}