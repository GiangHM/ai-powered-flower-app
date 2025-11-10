<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
  >
    <div class="bg-white rounded-lg shadow-xl max-w-4xl w-full mx-4 h-[80vh] flex flex-col">
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
              :disabled="isStreaming"
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
              :disabled="isStreaming"
              class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition disabled:bg-gray-100"
            />
          </div>
        </div>
      </div>

      <!-- Footer -->
      <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3">
        <button
          @click="closeModal"
          :disabled="isStreaming"
          class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 font-medium hover:bg-gray-100 transition disabled:opacity-50"
        >
          Close
        </button>
        <button
          @click="streamContent"
          :disabled="isStreaming || !researchPrompt || !writingPrompt"
          class="px-6 py-2 bg-blue-700 text-white rounded-lg font-medium hover:bg-blue-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ isStreaming ? 'Streaming...' : 'Stream' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { chatService } from '@/services/chat.service'

interface Props {
  isOpen: boolean
}

interface Emits {
  (e: 'close'): void
  (e: 'content', content: string): void
}

defineProps<Props>()
const emit = defineEmits<Emits>()

const researchPrompt = ref('')
const writingPrompt = ref('')
const messages = ref<Array<{ contextName: string; type: string; content: string }>>([])
const isStreaming = ref(false)

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
}

const streamContent = async () => {
  if (!researchPrompt.value.trim() || !writingPrompt.value.trim()) {
    alert('Please fill in both prompts')
    return
  }

  isStreaming.value = true
  messages.value = []

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
