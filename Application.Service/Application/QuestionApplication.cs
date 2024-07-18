using Application.Service.Interfaces;
using Domain.Entities;
using Domain.Service.Interfaces;
using Domain.Service.Service.ServiceAPI;
using Domain.ViewModel;

namespace Application.Service.Application
{
    public class QuestionApplication : IQuestionApplication
    {
        private readonly IQuestionService _questionService;

        public QuestionApplication(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public async Task DeleteQuestion(string id)
        {
            await _questionService.DeleteQuestion(id);
        }

        public async Task<Question> GetQuestionById(string id)
        {
            return await _questionService.GetQuestionById(id);
        }

        public async Task<Question> GetQuestionByIdSession(string id)
        {
            return await _questionService.GetQuestionByIdSession(id);
        }

        public async Task<List<Question>> GetQuestions()
        {
            return await _questionService.GetQuestions();
        }

        public async Task<Question> InsertQuestion(QuestionViewModel questionViewModel)
        {
            // tratar objeto null
            return await _questionService.InsertQuestion(questionViewModel);
        }

        public async Task ToAsk(string idQuestion)
        {
            await _questionService.ToAsk(idQuestion);
        }

        public async Task UpdateQuestion(string id, QuestionViewModel questionViewModel)
        {
            await _questionService.UpdateQuestion(id, questionViewModel);
        }
    }
}