    using UnityEngine;

    public class ObjectPlacementManager : Singleton<ObjectPlacementManager>
    {
        public GameObject objectToPlace; // Yerleştirilecek obje
        public GameObject objectPreview; // Objeyi önizlemek için kullanılacak obje
        public KeyCode placeKey = KeyCode.Mouse0; // Yerleştirme tuşu
        public KeyCode rotateKey = KeyCode.R; // Döndürme tuşu
        public KeyCode previewKey = KeyCode.P; // Preview oluşturma tuşu
        public string terrainTag = "Terrain"; // Terrain tag'i
        public LayerMask layerMask; // Raycast layer mask

        private GameObject objectPreviewInstance; // Oluşturulan önizleme objesi
        private GameObject currentPreviewObject; // Şu anda önizleme objesi olarak kullanılan obje
        private Quaternion rotation = Quaternion.identity; // Objeyi döndürmek için kullanılan dönüş
    private Camera playerCamera;
    public bool placeMode = false;
    public override void Awake()
    {

        playerCamera = GetComponentInChildren<Camera>();
         
        base.Awake();
    }
    void Update()
        {
       
        if (placeMode == true)
            {

            UpdatePreviewObject(); // Her güncellemede önizleme objesinin konumunu güncelle

            if (Input.GetKeyDown(rotateKey))
                {
                    RotatePreviewObjectClockwise();
                }
                if (Input.GetKeyDown(placeKey))
                {
                   // if (currentPreviewObject != null && !currentPreviewObject.GetComponent<PreviewObject>().isCollid)
                    //{
                        PlaceObject();
                   // }
                }
            }
       
       

         
        }

    void PlaceObject()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Kameranın bakış yönünde bir ray gönder ve çarptığı yeri kontrol et
        if (Physics.Raycast(ray, out hit, 5f, layerMask))
        {
            // Rayin belirli bir mesafeden sonra çarptığı yerin tag'ini kontrol et
            if (hit.collider.CompareTag(terrainTag))
            {
                // Terrain tag'ine sahip bir colliderın üzerine objeyi yerleştir
                float yOffset = objectToPlace.transform.localScale.y / 2; // Objeyi yere oturtmak için yüksekliğin yarısını kullan
                Vector3 spawnPosition = hit.point + Vector3.up * yOffset; // Yerleştirme konumunu hesapla
                                                                          //Instantiate(objectToPlace, spawnPosition, rotation);
                objectToPlace.transform.position = spawnPosition;
                objectToPlace.transform.rotation = rotation;
                objectToPlace.SetActive(true);
                // Önizleme objesini sil
                Destroy(objectPreviewInstance);
                placeMode = false;
            }
            else
            {
                Debug.Log("Yerleştirme yapılamadı. Lütfen Terrain tag'ine sahip bir yüzeye tıklayın.");
            }
        }
    }
    /*  void PlaceObject()
       {
           RaycastHit hit;
           Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

           // Kameranın bakış yönünde bir ray gönder ve çarptığı yeri kontrol et
           if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
           {
               // Rayin çarptığı yerin tag'ini kontrol et
               if (hit.collider.CompareTag(terrainTag))
               {
                   // Terrain tag'ine sahip bir colliderın üzerine objeyi yerleştir
                   float yOffset = objectToPlace.transform.localScale.y / 2; // Objeyi yere oturtmak için yüksekliğin yarısını kullan
                   Vector3 spawnPosition = hit.point + Vector3.up * yOffset; // Yerleştirme konumunu hesapla
                                                                             //Instantiate(objectToPlace, spawnPosition, rotation);
                   objectToPlace.transform.position = spawnPosition;
                   objectToPlace.transform.rotation = rotation;
                   objectToPlace.SetActive(true);
                   // Önizleme objesini sil
                   Destroy(objectPreviewInstance);
                   placeMode = false;
               }
               else
               {
                   Debug.Log("Yerleştirme yapılamadı. Lütfen Terrain tag'ine sahip bir yüzeye tıklayın.");
               }
           }
       }*/

    void RotatePreviewObjectClockwise()
        {
            rotation *= Quaternion.Euler(0, 10, 0); // Saat yönünde 10 derece döndür
            objectPreviewInstance.transform.rotation = rotation; // Preview objesini döndür
        }

        void UpdatePreviewObject()
        {
        if (objectPreviewInstance != null && objectPreviewInstance.activeSelf)
        {
            RaycastHit hit;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out hit, 5f, layerMask))
            {
                if (hit.collider.CompareTag(terrainTag))
                {
                    float yOffset = objectPreviewInstance.transform.localScale.y / 2;
                    Vector3 targetPosition = hit.point + Vector3.up * yOffset;

                    // Eğer mevcut pozisyon ile hedef pozisyon arasındaki mesafe belirli bir eşik değerinden fazlaysa,
                    // sadece o zaman pozisyon güncellemesi yap
                  
                        objectPreviewInstance.transform.position = targetPosition;
                        currentPreviewObject = objectPreviewInstance;
                    
                }
            }
        }
    }

        public void TogglePreviewObject()
        {

            // Eğer önizleme objesi oluşturulmamışsa veya görünür değilse
            if (objectPreviewInstance == null || !objectPreviewInstance.activeSelf)
            {
                // Önizleme objesini oluştur ve görünür yap
                if (objectPreviewInstance == null)
                {
                    objectPreviewInstance = Instantiate(objectPreview, Vector3.zero, Quaternion.identity);
                }
                objectPreviewInstance.SetActive(true);
                // Önizleme objesi görünürken klonlama işlemi yapma
                currentPreviewObject = objectPreviewInstance;
            }
            else
            {
                // Aksi takdirde görünürlüğünü değiştir
                objectPreviewInstance.SetActive(!objectPreviewInstance.activeSelf);
                // Eğer önizleme objesi devre dışı bırakılıyorsa, klonlama işlemi için null'a ayarla
                currentPreviewObject = null;
            }

        }

        // Gizmos çizmek için OnDrawGizmos() fonksiyonunu kullan
        void OnDrawGizmos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 10f);

            // Gizmos olarak küp çiz
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(ray.origin + ray.direction * 10f, Vector3.one);
        }
    }
