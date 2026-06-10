<template>
  <div class="card">
    <h2>Create Subscription</h2>

    <form @submit.prevent="handleSubmit">
      <div class="field">
        <label for="email">Email *</label>
        <input id="email" v-model="form.email" type="email" placeholder="you@example.com" />
      </div>

      <div class="field">
        <label for="city">City *</label>
        <input id="city" v-model="form.city" type="text" placeholder="Berlin" />
      </div>

      <div class="field">
        <label for="country">Country *</label>
        <input id="country" v-model="form.country" type="text" placeholder="DE" maxlength="2" />
      </div>

      <div class="field">
        <label for="zipCode">Zip Code</label>
        <input id="zipCode" v-model="form.zipCode" type="text" placeholder="10115 (optional)" />
      </div>

      <p v-if="error" class="message error">{{ error }}</p>
      <p v-if="success" class="message success">{{ success }}</p>

      <button type="submit" :disabled="loading">
        {{ loading ? 'Subscribing...' : 'Subscribe' }}
      </button>
    </form>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import { createSubscription } from '../services/api.js'

const form = reactive({ email: '', city: '', country: '', zipCode: '' })
const error = ref('')
const success = ref('')
const loading = ref(false)

async function handleSubmit() {
  error.value = ''
  success.value = ''

  if (!form.email || !form.city || !form.country) {
    error.value = 'Email, city and country are required.'
    return
  }

  loading.value = true
  try {
    const result = await createSubscription({
      email: form.email,
      city: form.city,
      country: form.country,
      zipCode: form.zipCode || null
    })
    success.value = `Subscribed successfully! ID: ${result.id}`
    form.email = ''
    form.city = ''
    form.country = ''
    form.zipCode = ''
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.card {
  background: var(--color-card);
  border-radius: 10px;
  padding: 2rem;
}

h2 {
  margin: 0 0 1.5rem;
  font-size: 1.2rem;
  color: var(--color-heading);
}

.field {
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
  margin-bottom: 1rem;
}

label {
  font-size: 0.85rem;
  color: var(--color-label);
}

input {
  padding: 0.6rem 0.8rem;
  border: 1px solid var(--color-border);
  border-radius: 6px;
  background: var(--color-input);
  color: var(--color-text);
  font-size: 0.95rem;
  outline: none;
  transition: border-color 0.2s;
}

input:focus {
  border-color: var(--color-accent);
}

button {
  width: 100%;
  padding: 0.7rem;
  background: var(--color-accent);
  color: #fff;
  border: none;
  border-radius: 6px;
  font-size: 1rem;
  cursor: pointer;
  margin-top: 0.5rem;
  transition: opacity 0.2s;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.message {
  margin: 0.5rem 0;
  padding: 0.6rem 0.8rem;
  border-radius: 6px;
  font-size: 0.9rem;
}

.error {
  background: #fde8e8;
  color: #c0392b;
}

.success {
  background: #e8f5e9;
  color: #2e7d32;
}
</style>
