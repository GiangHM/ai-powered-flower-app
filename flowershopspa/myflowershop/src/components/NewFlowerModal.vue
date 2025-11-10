<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50"
  >
    <div class="bg-white rounded-lg shadow-xl max-w-6xl w-full mx-4 max-h-screen overflow-y-auto">
      <!-- Header -->
      <div class="px-6 py-4 border-b border-gray-200 flex justify-between items-center sticky top-0 bg-white">
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

          <!-- Image URL -->
          <div class="mb-6">
            <label class="block text-sm font-semibold text-gray-700 mb-2">
              Image URL <span class="text-red-600">*</span>
            </label>
            <div class="flex gap-2">
              <input
                v-model="formData.imageUrl"
                type="url"
                placeholder="https://example.com/image.jpg"
                class="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
                required
              />
              <button
                type="button"
                @click="browseImage"
                class="px-4 py-2 bg-gray-200 text-gray-700 rounded-lg hover:bg-gray-300 transition font-medium"
              >
                Browse
              </button>
            </div>
            <!-- Image Preview -->
            <div v-if="imageReview" class="mt-3">
              <img
                :src="imageReview"
                alt="Preview"
                class="max-h-40 rounded-lg border border-gray-200"
                @error="imageError = true"
              />
              <p v-if="imageError" class="text-red-600 text-sm mt-2">Failed to load image</p>
            </div>
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
      <div class="px-6 py-4 border-t border-gray-200 bg-gray-50 flex justify-end gap-3 sticky bottom-0">
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
  </div>

  <!-- Hidden file input for image upload -->
  <input
    ref="fileInput"
    type="file"
    accept="image/*"
    class="hidden"
    @change="handleFileSelect"
  />

    <!-- Creative Writer Modal -->
  <CreativeWriterModal
    :isOpen="showCreativeWriter"
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

interface Props {
  isOpen: boolean
}

interface Emits {
  (e: 'close'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const fileInput = ref<HTMLInputElement | null>(null)
const isSubmitting = ref(false)
const imageError = ref(false)
const showCreativeWriter = ref(false)

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
  imageError.value = false
}

const browseImage = () => {
  fileInput.value?.click()
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      //Use later to upload to blob storage.
      //With MVP version, we focus on AI skills first
      imageReview.value = e.target?.result as string
      formData.value.imageUrl = file.name
      imageError.value = false
    }
    reader.readAsDataURL(file)

    // For demo, to display image on the shop, 
    // I should copy the image to appropriate folder.
    
  }
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
  console.log(showCreativeWriter)
}

const closeCreativeWriter = () => {
  showCreativeWriter.value = false
}

const handleAiContent = (content: string) => {
  // Append AI-generated content to description
  formData.value.description += (formData.value.description ? '\n\n' : '') + content
  closeCreativeWriter()
}
</script>
