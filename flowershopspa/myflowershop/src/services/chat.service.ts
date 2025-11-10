import {
  type AIChatMessage,
  AIChatProtocolClient,
} from "@microsoft/ai-chat-protocol";
import { API_CONFIG } from '@/config/api.configs'

class ChatService {
  private readonly baseUrl = API_CONFIG.baseURL
  private readonly chatEndpoint = `${this.baseUrl}api/Chat`

  /**
   * Stream chat content using Microsoft AI Chat Protocol
   * @param research - Research prompt for the agent
   * @param writing - Writing prompt for the article
   * @returns Async iterable of streaming responses
   */
  public async streamChat(research: string, writing: string): Promise<AsyncIterable<any>> {
    const client = new AIChatProtocolClient(this.chatEndpoint);

    const message: AIChatMessage = {
      role: "user",
      content: JSON.stringify({
        research,
        writing,
      }),
    }

    return client.getStreamedCompletion([message])
  }
}

export const chatService = new ChatService()
