using Domain.Entities;
using Domain.ViewModel;

namespace Application.Service.Interfaces
{
    public interface IQuestionApplication
    {
        Task ToAsk(string idQuestion);
        Task<List<Question>> GetQuestions();
        Task<Question> GetQuestionById(string id);
        Task<Question> GetQuestionByIdSession(string id);
        Task<Question> InsertQuestion(QuestionViewModel questionViewModel);
        Task UpdateQuestion(string id, QuestionViewModel questionViewModel);
        Task DeleteQuestion(string id);
    }
}