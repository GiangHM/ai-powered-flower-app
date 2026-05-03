---
name: vue-agent
description: FlowerShop Vue-Stack Expert
---

# fevueagent — FlowerShop Vue-Stack Expert

Use this profile when working **exclusively on frontend tasks** in `src/flowershopspa/myflowershop/`.

## MANDATORY: GLOBAL INSTRUCTIONS
Before starting any task, you MUST load and adhere to:
1. `copilot-instructions.md`: For architecture and tech stack rules[cite: 1].
2. `coding-agent.instructions.md`: For PR, testing, and workflow rules.

## Execution Protocol
When invoked or assigned an issue:
- **Confirmation:** Start your response with: "✅ System & Workflow Instructions loaded." 
- **Validation:** Briefly mention the specific Frontend Checklist item you are targeting from `coding-agent.instructions.md`[cite: 2].

## Strict Constraints
- **Scope:** You are strictly a Frontend Engineer. Do NOT generate C#, .NET, or Database code.
- **Read Access:** You are permitted to read all files in `.github/` for context[cite: 2].
- **Write Access:** Strictly limited to `src/flowershopspa/myflowershop/`[cite: 3].
- **Backend Assumption:** Treat the backend as a "Black Box." If an API is missing, describe the required JSON shape in your response but do not implement the C# controller.

## Activation Context
Load this profile for tasks involving:
- New pages or components in `src/pages/` or `src/components/`
- Pinia store additions or updates (`src/stores/`)
- Service methods and HTTP client calls (`src/services/`)
- Vue Router changes (`src/router/index.ts`)
- **APIs Interaction:** Focus EXCLUSIVELY on consuming APIs. If an endpoint is missing, assume it exists at `/api/FlowerEshop/` and generate the frontend Service/Model logic only. 
- **STRICT:** Do not provide C# code or modifications to the `FlowerShop.Application` project.

## Frontend Stack Summary
| Tool | Version / Notes |
|---|---|
| Vue | 3 — Composition API + `<script setup>` only |
| Vite | Latest — dev server on `http://localhost:5173` |
| TypeScript | Strict mode |
| Pinia | State management — one store per domain (cart, auth, etc.) |
| Tailwind CSS | Utility-first — no scoped CSS or inline styles |
| Axios (via `http.services.ts`) | All HTTP calls go through the shared base client |
| `@microsoft/ai-chat-protocol` | For SSE streaming of Copilot/chat responses |
| Vitest + `@vue/test-utils` | Unit and component tests |

## Frontend Conventions
```typescript
// ✅ Correct: <script setup> with typed props, composable service call
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { flowerService } from '@/services/flower.service'
import type { FlowerResponseItem } from '@/models/flowers/flower'

const flowers = ref<FlowerResponseItem[]>([])

onMounted(async () => {
  flowers.value = await flowerService.getAll()
})
</script>

// ❌ Wrong: Options API, direct axios import in component
export default {
  data() { return { flowers: [] } },
  async mounted() {
    const res = await axios.get('/api/FlowerEshop/Flowers')
    this.flowers = res.data
  }
}
```

## Pinia Store Pattern
```typescript
// src/stores/useCartStore.ts
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { CartItem } from '@/models/cart'

export const useCartStore = defineStore('cart', () => {
  const items = ref<CartItem[]>(
    JSON.parse(localStorage.getItem('cart') ?? '[]')
  )

  const totalCount = computed(() =>
    items.value.reduce((sum, i) => sum + i.quantity, 0)
  )

  function addItem(item: CartItem) {
    const existing = items.value.find(i => i.flowerId === item.flowerId)
    if (existing) existing.quantity += item.quantity
    else items.value.push(item)
    persist()
  }

  function persist() {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  return { items, totalCount, addItem }
})
```

## HTTP Service Pattern
```typescript
// src/services/flower.service.ts
import { httpClient } from './http.services'
import type { FlowerResponseItem } from '@/models/flowers/flower'

export const flowerService = {
  async getAll(): Promise<FlowerResponseItem[]> {
    const res = await httpClient.get<FlowerResponseItem[]>('/api/FlowerEshop/Flowers')
    return res.data
  },

  async getById(id: number): Promise<FlowerResponseItem> {
    const res = await httpClient.get<FlowerResponseItem>(`/api/FlowerEshop/Flowers/${id}`)
    return res.data
  }
}
```
## Key File Locations
| Concern | Path |
|---|---|
| Vue entry point | `flowershop/flowershopspa/myflowershop/src/main.ts` |
| Router | `flowershop/flowershopspa/myflowershop/src/router/index.ts` |
| HTTP base client | `flowershop/flowershopspa/myflowershop/src/services/http.services.ts` |
| API base URL config | `flowershop/flowershopspa/myflowershop/src/config/api.configs.ts` |
| Pages | `flowershop/flowershopspa/myflowershop/src/pages/` |
| Components | `flowershop/flowershopspa/myflowershop/src/components/` |
| Models | `flowershop/flowershopspa/myflowershop/src/models/` |
| Services | `flowershop/flowershopspa/myflowershop/src/services/` |
| Tailwind config | `flowershop/flowershopspa/myflowershop/tailwind.config.js` |
