<template>
  <!-- Floating toggle button -->
  <button
    @click="toggleChat"
    class="fixed bottom-6 right-6 z-50 w-14 h-14 rounded-full bg-pink-600 text-white shadow-lg hover:bg-pink-700 active:scale-95 transition-all flex items-center justify-center"
    :aria-label="isOpen ? 'Close chat' : 'Open sales chat'"
  >
    <!-- Chat bubble icon when closed -->
    <svg v-if="!isOpen" class="w-7 h-7" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path
        stroke-linecap="round"
        stroke-linejoin="round"
        stroke-width="2"
        d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"
      />
    </svg>
    <!-- X icon when open -->
    <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
    </svg>
  </button>

  <!-- Chat panel -->
  <Transition
    enter-active-class="transition ease-out duration-200"
    enter-from-class="opacity-0 translate-y-4 scale-95"
    enter-to-class="opacity-100 translate-y-0 scale-100"
    leave-active-class="transition ease-in duration-150"
    leave-from-class="opacity-100 translate-y-0 scale-100"
    leave-to-class="opacity-0 translate-y-4 scale-95"
  >
    <div
      v-if="isOpen"
      class="fixed bottom-24 right-6 z-50 w-80 sm:w-96 flex flex-col rounded-2xl shadow-2xl bg-white border border-gray-200 overflow-hidden"
      style="height: 28rem;"
    >
      <!-- Header -->
      <div class="flex items-center gap-3 px-4 py-3 bg-pink-600 text-white shrink-0">
        <div class="w-8 h-8 rounded-full bg-white/20 flex items-center justify-center">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
          </svg>
        </div>
        <div class="flex-1 min-w-0">
          <p class="font-semibold text-sm leading-tight">Flower Shop Assistant</p>
          <p class="text-xs text-pink-200 leading-tight">Ask me about flowers 🌸</p>
        </div>
        <button @click="clearMessages" title="Clear conversation" class="text-pink-200 hover:text-white transition">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
          </svg>
        </button>
      </div>

      <!-- Messages list -->
      <div ref="messagesContainer" class="flex-1 overflow-y-auto px-4 py-3 space-y-3 bg-gray-50">
        <!-- Welcome message when no conversation yet -->
        <div v-if="messages.length === 0" class="text-center py-6">
          <div class="text-3xl mb-2">🌷</div>
          <p class="text-sm text-gray-500">Hi! I'm your flower shop assistant.</p>
          <p class="text-sm text-gray-500">Ask me to find flowers for any occasion!</p>
        </div>

        <!-- Message bubbles -->
        <div
          v-for="msg in messages"
          :key="msg.id"
          :class="msg.role === 'user' ? 'flex justify-end' : 'flex justify-start'"
        >
          <!-- Assistant avatar -->
          <div v-if="msg.role === 'assistant'" class="w-6 h-6 rounded-full bg-pink-100 flex items-center justify-center shrink-0 mr-2 mt-1">
            <span class="text-xs">🌸</span>
          </div>

          <div
            :class="[
              'max-w-[85%] rounded-2xl px-3 py-2 text-sm leading-relaxed shadow-sm',
              msg.role === 'user'
                ? 'bg-pink-600 text-white rounded-br-sm'
                : 'bg-white text-gray-800 rounded-bl-sm border border-gray-100'
            ]"
          >
            <!-- Streaming indicator for the in-progress assistant message -->
            <template v-if="msg.isStreaming && !msg.content">
              <span class="inline-flex items-center gap-1">
                <span class="w-1.5 h-1.5 rounded-full bg-gray-400 animate-bounce" style="animation-delay: 0ms" />
                <span class="w-1.5 h-1.5 rounded-full bg-gray-400 animate-bounce" style="animation-delay: 150ms" />
                <span class="w-1.5 h-1.5 rounded-full bg-gray-400 animate-bounce" style="animation-delay: 300ms" />
              </span>
            </template>

            <!-- Parsed message content: plain text + flower product links -->
            <template v-else>
              <template v-for="(segment, idx) in parseMessageSegments(msg.content)" :key="idx">
                <router-link
                  v-if="segment.type === 'link' && segment.href"
                  :to="segment.href"
                  class="inline-flex items-center gap-1 font-medium underline decoration-dotted hover:no-underline"
                  :class="msg.role === 'user' ? 'text-pink-100 hover:text-white' : 'text-pink-600 hover:text-pink-800'"
                >
                  <svg class="w-3 h-3 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>{{ segment.text }}
                </router-link>
                <span v-else class="whitespace-pre-wrap">{{ segment.text }}</span>
              </template>
              <!-- Blinking cursor while streaming -->
              <span v-if="msg.isStreaming" class="inline-block w-0.5 h-4 bg-gray-400 ml-0.5 animate-pulse align-middle" />
            </template>
          </div>
        </div>
      </div>

      <!-- Error banner -->
      <div v-if="errorMessage" class="px-4 py-2 bg-red-50 border-t border-red-100 text-xs text-red-600 flex items-center gap-2 shrink-0">
        <svg class="w-4 h-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4v.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        {{ errorMessage }}
      </div>

      <!-- Input area -->
      <div class="px-3 py-3 border-t border-gray-200 bg-white shrink-0">
        <form @submit.prevent="sendMessage" class="flex items-end gap-2">
          <textarea
            ref="inputRef"
            v-model="inputText"
            placeholder="Ask about flowers…"
            rows="1"
            :disabled="isStreaming"
            @keydown.enter.exact.prevent="sendMessage"
            @input="autoResize"
            class="flex-1 resize-none rounded-xl border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-pink-400 focus:border-transparent disabled:bg-gray-50 disabled:text-gray-400 max-h-24 overflow-y-auto leading-relaxed"
          />
          <button
            type="submit"
            :disabled="isStreaming || !inputText.trim()"
            class="w-9 h-9 rounded-xl bg-pink-600 text-white flex items-center justify-center hover:bg-pink-700 active:scale-95 transition-all disabled:opacity-40 disabled:cursor-not-allowed shrink-0"
            aria-label="Send message"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
            </svg>
          </button>
        </form>
      </div>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'
import { chatService, type SalesChatHistoryEntry } from '@/services/chat.service'

interface ChatMessage {
  id: string
  role: 'user' | 'assistant'
  content: string
  isStreaming?: boolean
}

interface MessageSegment {
  type: 'text' | 'link'
  text: string
  href?: string
}

// Internal markdown link pattern: [text](/flowers/123)
const FLOWER_LINK_RE = /\[([^\]]+)\]\((\/flowers\/\d+)\)/g

// Stable session ID for the lifetime of the component
const sessionId = crypto.randomUUID()

const isOpen = ref(false)
const messages = ref<ChatMessage[]>([])
const inputText = ref('')
const isStreaming = ref(false)
const errorMessage = ref<string | null>(null)
const messagesContainer = ref<HTMLElement | null>(null)
const inputRef = ref<HTMLTextAreaElement | null>(null)

function toggleChat() {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => inputRef.value?.focus())
  }
}

function clearMessages() {
  if (!isStreaming.value) {
    messages.value = []
    errorMessage.value = null
  }
}

/**
 * Parses message content that may contain markdown flower links
 * [Flower Name](/flowers/42) into renderable segments.
 */
function parseMessageSegments(content: string): MessageSegment[] {
  if (!content) return []

  const segments: MessageSegment[] = []
  let lastIndex = 0
  let match: RegExpExecArray | null

  FLOWER_LINK_RE.lastIndex = 0
  while ((match = FLOWER_LINK_RE.exec(content)) !== null) {
    if (match.index > lastIndex) {
      segments.push({ type: 'text', text: content.slice(lastIndex, match.index) })
    }
    segments.push({ type: 'link', text: match[1] ?? match[0], href: match[2] })
    lastIndex = match.index + match[0].length
  }

  if (lastIndex < content.length) {
    segments.push({ type: 'text', text: content.slice(lastIndex) })
  }

  return segments.length > 0 ? segments : [{ type: 'text', text: content }]
}

function autoResize(event: Event) {
  const el = event.target as HTMLTextAreaElement
  el.style.height = 'auto'
  el.style.height = `${Math.min(el.scrollHeight, 96)}px`
}

async function scrollToBottom() {
  await nextTick()
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

async function sendMessage() {
  const text = inputText.value.trim()
  if (!text || isStreaming.value) return

  errorMessage.value = null

  // Add user message to conversation
  messages.value.push({ id: crypto.randomUUID(), role: 'user', content: text })
  inputText.value = ''

  // Reset textarea height
  if (inputRef.value) {
    inputRef.value.style.height = 'auto'
  }

  await scrollToBottom()

  // Placeholder assistant message that streams into
  const assistantMsg: ChatMessage = {
    id: crypto.randomUUID(),
    role: 'assistant',
    content: '',
    isStreaming: true
  }
  messages.value.push(assistantMsg)
  await scrollToBottom()

  isStreaming.value = true

  try {
    // Build history from all settled messages (exclude the streaming placeholder)
    const history: SalesChatHistoryEntry[] = messages.value
      .filter(m => !m.isStreaming && m.content)
      .map(m => ({ role: m.role, content: m.content }))

    for await (const delta of chatService.streamSalesChat(sessionId, text, history)) {
      if (delta.delta?.content) {
        assistantMsg.content += delta.delta.content
        await scrollToBottom()
      }
    }
  } catch (err: unknown) {
    console.error('[ChatWidget] stream error:', err)
    errorMessage.value = 'Sorry, something went wrong. Please try again.'
    assistantMsg.content = 'Sorry, I encountered an error. Please try again.'
  } finally {
    assistantMsg.isStreaming = false
    isStreaming.value = false
    await scrollToBottom()
  }
}
</script>
