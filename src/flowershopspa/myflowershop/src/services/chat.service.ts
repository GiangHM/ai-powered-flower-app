import {
  type AIChatMessage,
  AIChatProtocolClient,
} from "@microsoft/ai-chat-protocol";
import { API_CONFIG } from '@/config/api.configs'

/** Shape of a history entry sent to the sales endpoint */
export interface SalesChatHistoryEntry {
  role: 'user' | 'assistant'
  content: string
}

/** A single streamed delta from the sales endpoint */
export interface SalesChatDelta {
  delta?: {
    content?: string
    role?: string
    context?: {
      name?: string
    }
  }
  sessionState?: string
}

class ChatService {
  private readonly baseUrl = API_CONFIG.baseURL
  private readonly chatEndpoint = `${this.baseUrl}api/Chat/writer`
  private readonly salesEndpoint = `${this.baseUrl}api/Chat/sales/stream`

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

  /**
   * Stream a sales assistant response from POST /api/Chat/sales/stream.
   * Sends { sessionId, message, history } and parses the ndjson response stream.
   * @param sessionId - Stable session ID for conversation continuity
   * @param message - The latest user message text
   * @param history - Prior conversation turns
   * @returns Async generator yielding parsed AIChatCompletionDelta objects
   */
  public async *streamSalesChat(
    sessionId: string,
    message: string,
    history: SalesChatHistoryEntry[]
  ): AsyncGenerator<SalesChatDelta> {
    const token = localStorage.getItem('auth_token')
    const headers: Record<string, string> = { 'Content-Type': 'application/json' }
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(this.salesEndpoint, {
      method: 'POST',
      headers,
      body: JSON.stringify({ sessionId, message, history }),
    })

    if (!response.ok || !response.body) {
      throw new Error(`Sales chat request failed: ${response.status} ${response.statusText}`)
    }

    const reader = response.body.getReader()
    const decoder = new TextDecoder()
    let buffer = ''

    try {
      while (true) {
        const { done, value } = await reader.read()
        if (done) break

        buffer += decoder.decode(value, { stream: true })
        const lines = buffer.split('\n')
        // Keep the last (potentially incomplete) line in the buffer
        buffer = lines.pop() ?? ''

        for (const line of lines) {
          const trimmed = line.trim()
          if (!trimmed) continue
          try {
            yield JSON.parse(trimmed) as SalesChatDelta
          } catch {
            // Skip malformed lines
          }
        }
      }

      // Flush any remaining content
      if (buffer.trim()) {
        try {
          yield JSON.parse(buffer.trim()) as SalesChatDelta
        } catch {
          // Skip
        }
      }
    } finally {
      reader.releaseLock()
    }
  }
}

export const chatService = new ChatService()
