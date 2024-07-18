using Domain.Entities;
using Domain.ViewModel;

namespace Domain.Service.Interfaces
{
    public interface IQuestionService
    {
        Task<Conversation> ToAsk(string idQuestion);
        Task<List<Question>> GetQuestions();
        Task<Question> GetQuestionById(string id);
        Task<Question> GetQuestionByIdSession(string id);
        Task<Question> InsertQuestion(QuestionViewModel questionViewModel);
        Task UpdateQuestion(string id, QuestionViewModel questionViewModel);
        Task DeleteQuestion(string id);

    }
}