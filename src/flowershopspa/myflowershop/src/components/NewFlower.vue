<template>
  <div class="bg-white rounded-lg shadow">
    <!-- Header -->
    <div class="px-6 py-4 border-b border-gray-200 flex justify-between items-center bg-white">
      <h2 class="text-xl font-bold text-gray-900">New Flower</h2>
      <button
        @click="closeModal"
        class="text-gray-400 hover:text-gray-600 text-2xl leading-none"
      >
        ×
      </button>
    </div>

    <!-- Form Content -->
    <div class="px-6 py-6">
      <form>
        <!-- Flower Name -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Flower Name <span class="text-red-600">*</span>
          </label>
          <input
            v-model="formData.name"
            type="text"
            placeholder="Enter flower name"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            required
          />
        </div>

        <!-- Description -->
        <div class="mb-6">
          <div class="flex justify-between items-center mb-2">
            <label class="block text-sm font-semibold text-gray-700">
              Description
            </label>
            <!-- Add creative writer button next to description label -->
            <button
              type="button"
              @click="openCreativeWriter"
              class="flex items-center gap-1 px-3 py-1 text-sm bg-purple-100 text-purple-700 rounded-lg hover:bg-purple-200 transition font-medium"
            >
              <span>✨</span> AI Writer
            </button>
          </div>
          <textarea
            v-model="formData.description"
            placeholder="Enter flower description"
            rows="4"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition resize-none"
          />
        </div>

        <!-- Image Upload -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Flower Image <span class="text-red-600">*</span>
          </label>

          <!-- Drop Zone -->
          <div
            class="border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition"
            :class="isDragging ? 'border-blue-500 bg-blue-50' : 'border-gray-300 hover:border-blue-400'"
            @dragover.prevent="isDragging = true"
            @dragleave.prevent="isDragging = false"
            @drop.prevent="onDrop"
            @click="fileInput?.click()"
          >
            <div v-if="isUploading" class="flex flex-col items-center gap-2 text-gray-500">
              <svg class="animate-spin h-8 w-8 text-blue-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
              </svg>
              <span class="text-sm">Uploading…</span>
            </div>
            <div v-else-if="isIdentifying" class="flex flex-col items-center gap-2 text-purple-500">
              <svg class="animate-spin h-8 w-8 text-purple-500" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v8z"/>
              </svg>
              <img v-if="imageReview" :src="imageReview" alt="Preview" class="max-h-40 rounded-lg border border-gray-200 opacity-70" />
              <span class="text-sm font-medium">🔍 Identifying flower with AI…</span>
            </div>
            <div v-else-if="imageReview" class="flex flex-col items-center gap-2">
              <img :src="imageReview" alt="Preview" class="max-h-40 rounded-lg border border-gray-200" />
              <span class="text-xs text-gray-400">Click or drop to replace</span>
            </div>
            <div v-else class="flex flex-col items-center gap-2 text-gray-400">
              <svg class="h-10 w-10" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M4 16v2a2 2 0 002 2h12a2 2 0 002-2v-2M12 12V4m0 0L8 8m4-4l4 4" />
              </svg>
              <span class="text-sm font-medium">Drag &amp; drop an image here</span>
              <span class="text-xs">or click to browse</span>
              <span class="text-xs">JPEG · PNG · GIF · WEBP — max 10 MB</span>
            </div>
          </div>
          <p v-if="uploadError" class="text-red-600 text-sm mt-2">{{ uploadError }}</p>
        </div>

        <!-- Category -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Category <span class="text-red-600">*</span>
          </label>
          <VueSelect
            v-model="formData.categoryId"
            :options="categories"
            :reduce="(option:any) => option.id"
            label="categoryName"
            placeholder="Select a category"
            class="vue-select-wrapper"
            :clearable="false"
          />
        </div>

        <!-- Unit Price -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Unit Price <span class="text-red-600">*</span>
          </label>
          <input
            v-model.number="formData.unitPrice"
            type="number"
            placeholder="0"
            min="0"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            required
          />
        </div>

        <!-- Unit Currency -->
        <div class="mb-6">
          <label class="block text-sm font-semibold text-gray-700 mb-2">
            Unit Currency <span class="text-red-600">*</span>
          </label>
          <input
            v-model="formData.unitCurrency"
            type="text"
            placeholder="VND"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            required
          />
        </div>
      </form>
    </div>

    <!-- Footer -->
    <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3">
      <button
        @click="closeModal"
        class="px-6 py-2 border border-gray-300 rounded-lg text-gray-700 font-medium hover:bg-gray-100 transition"
      >
        Cancel
      </button>
      <button
        @click="createFlower()"
        :disabled="isSubmitting"
        class="px-6 py-2 bg-blue-700 text-white rounded-lg font-medium hover:bg-blue-800 transition disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {{ isSubmitting ? 'Saving...' : 'Save' }}
      </button>
    </div>
  </div>

  <!-- Hidden file input for image upload -->
  <input
    ref="fileInput"
    type="file"
    accept="image/*"
    class="hidden"
    @change="handleFileSelect"
  />

  <!-- Creative Writer Modal (keeps popup style) -->
  <CreativeWriterModal
    :isOpen="showCreativeWriter"
    :initialResearchPrompt="initialResearchPrompt"
    :initialWritingPrompt="initialWritingPrompt"
    :isIdentifying="isIdentifying"
    @close="closeCreativeWriter"
    @content="handleAiContent"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { CreateFlowerFormDto } from '@/models/flowers/flower'
import { flowerService } from '@/services/flower.service'
import VueSelect from 'vue-select'
import 'vue-select/dist/vue-select.css'
import CreativeWriterModal from '@/components/CreativeWritterModal.vue'

interface Emits {
  (e: 'close'): void
}

const emit = defineEmits<Emits>()

const fileInput = ref<HTMLInputElement | null>(null)
const isSubmitting = ref(false)
const isDragging = ref(false)
const isUploading = ref(false)
const uploadError = ref<string>()
const showCreativeWriter = ref(false)
const isIdentifying = ref(false)
const initialResearchPrompt = ref('')
const initialWritingPrompt = ref('')

const categories = [
    { categoryName: 'Beautiful bouquet of flowers', id: 1 },
    { categoryName: 'Garden style', id: 2 },
    { categoryName: 'Tulip flowers', id: 3 }]

const formData = ref<CreateFlowerFormDto>({
  name: '',
  description: '',
  imageUrl: '',
  categoryId: 0,
  unitPrice: 0,
  unitCurrency: 'VND'
})

const imageReview = ref<string>()

const closeModal = () => {
    resetForm()
    emit('close')
}
const resetForm = () => {
  formData.value = {
    name: '',
    description: '',
    imageUrl: '',
    categoryId: 0,
    unitPrice: 0,
    unitCurrency: 'VND'
  }
  imageReview.value = undefined
  uploadError.value = undefined
  initialResearchPrompt.value = ''
  initialWritingPrompt.value = ''
}

async function handleFileUpload(file: File) {
  uploadError.value = undefined
  isUploading.value = true
  try {
    const url = await flowerService.uploadImage(file)
    formData.value.imageUrl = url
    imageReview.value = url

    // After upload succeeds, identify the flower with GPT-4o vision
    isIdentifying.value = true
    try {
      const result = await flowerService.describeImage(file)
      formData.value.name = result.commonName
      
      initialResearchPrompt.value =
        `Research information about flower name: "${result.commonName}", flower type: "${result.flowerType}", notable characteristics: "${result.notableCharacteristics}": its origin, symbolic meaning, seasonal availability, and key appeal for flower shop customers.`
      initialWritingPrompt.value =
        `Write a compelling marketing description for flower name: "${result.commonName}", flower type: "${result.flowerType}", notable characteristics: "${result.notableCharacteristics}", based on the research. Keep it between 150–250 words, evoke emotion and sensory details.`
    } catch {
      // Identification failure is non-critical; inform user and let them fill prompts manually
      uploadError.value = 'Image uploaded but automatic flower identification failed. You can still fill in the AI Writer prompts manually.'
      setTimeout(() => { uploadError.value = undefined }, 5000)
    } finally {
      isIdentifying.value = false
    }
  } catch (e: any) {
    uploadError.value = e?.response?.data ?? e?.message ?? 'Upload failed'
  } finally {
    isUploading.value = false
  }
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) handleFileUpload(file)
}

const onDrop = (event: DragEvent) => {
  isDragging.value = false
  const file = event.dataTransfer?.files?.[0]
  if (file) handleFileUpload(file)
}

async function createFlower()
{
  try {
    var inputData = formData.value
    const newFlower = await flowerService.createFlower({
      name: inputData.name,
      description: inputData.description,
      imageUrl: inputData.imageUrl,
      categoryId: inputData.categoryId,
      unitPrice: inputData.unitPrice,
      unitCurrency: inputData.unitCurrency
    })

    emit('close')
   
  } catch (error) {
    console.error('Error creating flower:', error)
    alert('Failed to create flower')
  }
}

const openCreativeWriter = () => {
  showCreativeWriter.value = true
}

const closeCreativeWriter = () => {
  showCreativeWriter.value = false
}

const handleAiContent = (content: string) => {
  // Replace description with AI-generated content
  formData.value.description = content
  closeCreativeWriter()
}
</script>