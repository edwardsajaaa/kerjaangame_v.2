using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizContainerManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizStation
    {
        public string stationName; // Pos Soal 1, Pos Soal 2, dll
        public QuizManager quizManager;
        public List<QuizManager.Question> questions = new List<QuizManager.Question>();
    }

    [SerializeField] private List<QuizStation> quizStations = new List<QuizStation>();

    void Start()
    {
        // Setup setiap station dengan pertanyaannya sendiri
        for (int i = 0; i < quizStations.Count; i++)
        {
            QuizStation station = quizStations[i];
            if (station.quizManager != null)
            {
                // Clear pertanyaan lama
                station.quizManager.ClearQuestions();
                
                // Tambah pertanyaan dari station ini
                foreach (var question in station.questions)
                {
                    station.quizManager.AddQuestion(question.questionText, question.answers, question.correctAnswerIndex);
                }
                
                Debug.Log($"[{station.stationName}] Setup dengan {station.questions.Count} pertanyaan");
            }
        }
    }

    // Buat QuizStation baru
    public void AddQuizStation(string stationName, QuizManager quizManager)
    {
        QuizStation newStation = new QuizStation
        {
            stationName = stationName,
            quizManager = quizManager,
            questions = new List<QuizManager.Question>()
        };
        quizStations.Add(newStation);
    }

    // Cari station berdasarkan nama
    public QuizStation GetStationByName(string stationName)
    {
        return quizStations.Find(s => s.stationName == stationName);
    }
}
