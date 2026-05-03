<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-opacity-60 backdrop-blur-sm flex items-center justify-center z-50"
  >
    <div class="bg-white border-3 border-blue-500 rounded-lg shadow-xl max-w-4xl w-full mx-4 h-[80vh] flex flex-col">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 flex justify-between items-center">
        <h2 class="text-xl font-bold text-gray-900">Creative Writer Agent</h2>
        <button
          @click="closeModal"
          class="text-gray-400 hover:text-gray-600 text-2xl leading-none"
        >
          ×
        </button>
      </div>

      <!-- Chat Content -->
      <div class="flex-1 overflow-y-auto px-6 py-6 space-y-4">
        <!-- Identifying indicator -->
        <div v-if="isIdentifying" class="flex items-center gap-3 p-4 bg-purple-50 border border-purple-200 rounded-lg">
          <svg class="animate-spin h-5 w-5 text-purple-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
          </svg>
          <span class="text-sm text-purple-700 font-medium">Identifying flower from image…</span>
        </div>

        <!-- Messages -->
        <div
          v-for="(message, index) in messages"
          :key="index"
          :class="[
            'rounded-lg p-4 mb-4',
            message.type === 'researcher'
              ? 'bg-green-100 border border-green-200'
              : 'bg-amber-100 border border-amber-200'
          ]"
        >
          <div class="text-sm font-semibold mb-2">
            <span
              :class="
                message.type === 'researcher' ? 'text-green-800' : 'text-amber-800'
              "
            >
              {{ message.contextName || 'Agent' }}:
            </span>
          </div>
          <div
            :class="
              message.type === 'researcher' ? 'text-green-900' : 'text-amber-900'
            "
            class="text-sm whitespace-pre-wrap leading-relaxed"
          >
            {{ message.content }}
          </div>
        </div>

        <!-- Loading indicator -->
        <div v-if="isStreaming" class="text-center py-4">
          <div class="inline-flex items-center space-x-2">
            <div class="w-2 h-2 bg-blue-500 rounded-full animate-bounce"></div>
            <div class="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
            <div class="w-2 h-2 bg-blue-500 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
          </div>
        </div>
      </div>

      <!-- Input Form -->
      <div class="px-6 py-4 border-t border-gray-200 bg-gray-50">
        <div class="space-y-4">
          <!-- Research Input -->
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">
              Research Prompt
            </label>
            <textarea
              v-model="researchPrompt"
              placeholder="e.g., Can you find a flower for best friend in 2025?"
              rows="2"
              :disabled="isStreaming || isIdentifying"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition disabled:bg-gray-100"
            />
          </div>

          <!-- Writing Input -->
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">
              Writing Prompt
            </label>
            <textarea
              v-model="writingPrompt"
              placeholder="e.g., Write a fun and engaging article that includes the research result. The article should be between 200 to 400 words."
              rows="2"
              :disabled="isStreaming || isIdentifying"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition disabled:bg-gray-100"
            />
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-between items-center">
        <!-- Use Content button (shown after streaming completes with writer output) -->
        <div>
          <button
            v-if="writerContent && !isStreaming"
            @click="useWriterContent"
            class="px-5 py-2 bg-green-600 text-white rounded-lg font-medium hover:bg-green-700 transition flex items-center gap-2"
          >
            <span>✅</span> Use This Content
          </button>
        </div>

        <div class="flex gap-3">
          <button
            @click="closeModal"
            :disabled="isStreaming"
            class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 font-medium hover:bg-gray-100 transition disabled:opacity-50"
          >
            Close
          </button>
          <button
            @click="streamContent"
            :disabled="isStreaming || isIdentifying || !researchPrompt || !writingPrompt"
            class="px-6 py-2 bg-blue-700 text-white rounded-lg font-medium hover:bg-blue-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ isStreaming ? 'Streaming...' : 'Stream' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { chatService } from '@/services/chat.service'

interface Props {
  isOpen: boolean
  /** Pre-filled research prompt (e.g., from GPT-4o flower identification) */
  initialResearchPrompt?: string
  /** Pre-filled writing prompt */
  initialWritingPrompt?: string
  /** Whether the parent is still identifying the flower from the image */
  isIdentifying?: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'content', content: string): void
}

const props = withDefaults(defineProps<Props>(), {
  initialResearchPrompt: '',
  initialWritingPrompt: '',
  isIdentifying: false,
})
const emit = defineEmits<Emits>()

const researchPrompt = ref(props.initialResearchPrompt)
const writingPrompt = ref(props.initialWritingPrompt)
const messages = ref<Array<{ contextName: string; type: string; content: string }>>([])
const isStreaming = ref(false)
const writerContent = ref('')

// Keep prompts in sync when parent updates initial values (e.g. after identification)
watch(() => props.initialResearchPrompt, (val) => {
  researchPrompt.value = val
})
watch(() => props.initialWritingPrompt, (val) => {
  writingPrompt.value = val
})

const closeModal = () => {
  if (!isStreaming.value) {
    resetForm()
    emit('close')
  }
}

const resetForm = () => {
  researchPrompt.value = ''
  writingPrompt.value = ''
  messages.value = []
  writerContent.value = ''
}

const useWriterContent = () => {
  emit('content', writerContent.value)
}

const streamContent = async () => {
  if (!researchPrompt.value.trim() || !writingPrompt.value.trim()) {
    alert('Please fill in both prompts')
    return
  }

  isStreaming.value = true
  messages.value = []
  writerContent.value = ''

  try {
    const result = await chatService.streamChat(researchPrompt.value, writingPrompt.value)

    for await (const response of result) {
      if (response.delta?.content) {
        const contextName = response.delta.context?.name || 'Agent'
        const type = contextName.toLowerCase().includes('researcher') ? 'researcher' : 'writer'
        const content = response.delta.content

        // Find or create message for this context
        let message = messages.value.find(m => m.contextName === contextName && m.type === type)

        if (!message) {
          message = {
            content: '',
            contextName,
            type
          }
          messages.value.push(message)
        }

        message.content += content

        // Accumulate writer content for the "Use Content" button
        if (type === 'writer') {
          writerContent.value += content
        }
      }
    }
  } catch (error) {
    console.error('Stream error:', error)
    alert('Error streaming content. Please try again.')
  } finally {
    isStreaming.value = false
  }
}
</script>

