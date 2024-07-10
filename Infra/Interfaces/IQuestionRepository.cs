using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Interfaces
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetQuestionsAsync();
        Task<Question> GetQuestionByIdAsync(string questionId);
        Task<Question> InsertQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(Question question);
        Task<Question> GetQuestionByIdSessionAsync(string idSession);
    }
}