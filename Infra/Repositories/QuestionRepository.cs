using Domain.Entities;
using Domain.ViewModel;
using Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ContextDatabase _contextDatabase;

        public QuestionRepository(ContextDatabase contextDatabase)
        {
            _contextDatabase = contextDatabase;
        }

        public async Task<List<Question>> GetQuestionsAsync()
        {
            return await _contextDatabase.Questions.ToListAsync();
        }

        public async Task<Question> InsertQuestionAsync(Question question)
        {
            var result = await _contextDatabase.Questions.AddAsync(question);
            await _contextDatabase.SaveChangesAsync();
            return result.Entity;
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _contextDatabase.Questions.Update(question);
            await _contextDatabase.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Question question)
        {
            _contextDatabase.Questions.Remove(question);
            await _contextDatabase.SaveChangesAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(string questionId)
        {
            return await _contextDatabase.Questions
                .Include(q => q.Conversation)
                .FirstOrDefaultAsync(x => x.Id == questionId);
        }

        public async Task<Question> GetQuestionByIdSessionAsync(string idSession)
        {
            return await _contextDatabase.Questions
                .Include(q => q.Session)
                    .ThenInclude(s => s.Conversations)
                        .ThenInclude(c => c.Response)
                        .ThenInclude(r => r.Sources)
                .FirstOrDefaultAsync(x => x.IdSession == idSession);
        }
    }
}